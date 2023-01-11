using Clarity.Commands;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;

namespace Clarity.Tests
{
    [TestFixture]
    public class TrackingModelTests
    {
        [SetUp]
        public void Init()
        {
            ServiceManager.Default.UnregisterAll();
            ServiceManager.Default.Register<ICommandBuilder, DefaultCommandBuilder>();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestChangeThrowsExceptionWhenBeginEditHasNotBeenCalled()
        {
            var model = new TestTrackedModel(false, false);
            model.FirstName = "Wilma";
        }

        [Test]
        public void TestDataCanBeInitializedWithoutCallingBeginEdit()
        {
            var model = new TestTrackedModel(false, false);
            using (var context = model.CreateInitializationContext())
            {
                model.FirstName = "Wilma";
            }

            Assert.AreEqual("Wilma", model.FirstName);
            Assert.IsFalse(model.IsChanged);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBeginEditFailsWhenInitializing()
        {
            var model = new TestTrackedModel(false, false);
            using (var context = model.CreateInitializationContext())
            {
                model.BeginEdit();
            }
        }

        [Test]
        public void TestDataCanBeChangedAfterCallingBeginEdit()
        {
            var model = new TestTrackedModel(false, false);
            model.BeginEdit();
            model.FirstName = "Wilma";

            Assert.IsTrue(model.IsChanged);

            model.EndEdit();
            Assert.IsFalse(model.IsChanged);
        }

        [Test]
        public void IsReadonly_AfterCallingBeginEdit_ReturnsFalse()
        {
            var model = new TestTrackedModel(false, false);
            model.BeginEdit();
            Assert.IsFalse(model.IsReadOnly);
        }

        [Test]
        public void TestDataCanBeRolledBackWhenChangesAreCancelled()
        {
            var model = new TestTrackedModel(true, false);
            using (var context = model.CreateInitializationContext())
            {
                model.Items.Add("Barney");
                model.Items.Add("Rubble");
            }
            model.BeginEdit();
            model.FirstName = "Wilma";
            model.Items.Add("Test");

            Assert.IsTrue(model.IsChanged);
            Assert.AreEqual(3, model.Items.Count);
            model.CancelEdit();

            Assert.IsFalse(model.IsChanged);
            Assert.AreEqual("Fred", model.FirstName);
            Assert.AreEqual(2, model.Items.Count);
        }

        [Test]
        public void TestCollectionChangesAreIgnored()
        {
            var model = new TestTrackedModel(false, false);
            using (var context = model.CreateInitializationContext())
            {
                model.Items.Add("Barney");
                model.Items.Add("Rubble");
            }
            model.BeginEdit();
            model.FirstName = "Wilma";
            model.Items.Add("Test");

            Assert.IsTrue(model.IsChanged);
            Assert.AreEqual(3, model.Items.Count);
            model.CancelEdit();

            Assert.IsFalse(model.IsChanged);
            Assert.AreEqual("Fred", model.FirstName);
            Assert.AreEqual(3, model.Items.Count);
        }

        [Test]
        public void TestChangesAreCleared()
        {
            var model = new TestTrackedModel(false, false);
            model.BeginEdit();
            model.FirstName = "Wilma";
            model.ClearChanges();
            Assert.IsFalse(model.IsChanged);
        }

        [Test]
        public void TestBeginEditWorksFromConstructor()
        {
            var model = new TestTrackedModel(false, true);
            model.FirstName = "Elma";
            model.LastName = "Fudd";
            var changes = model.GetChanges();

            Assert.IsTrue(changes.ContainsKey("FirstName"));
            Assert.AreEqual(2, changes.Count);

            model.EndEdit();

            changes = model.GetChanges();
            Assert.AreEqual(0, changes.Count);
        }

        [Test]
        public void TestChangedPropertiesAreRecored()
        {
            var model = new TestTrackedModel(false, false);
            model.BeginEdit();
            model.FirstName = "Wilma";
            Assert.AreEqual(1, model.GetChangedProperties().Count);

            model.AcceptChanges();
            Assert.AreEqual(0, model.GetChangedProperties().Count);
        }

        [Test]
        public void TestChangesToFirstNameIsUntracked()
        {
            var model = new TestTrackedModel(false, false);
            model.IgnoreFirstname();

            model.BeginEdit();
            model.FirstName = "Joe";
            model.LastName = "Bloggs";
            Assert.AreEqual(1, model.GetChangedProperties().Count);

            model.AcceptChanges();
            Assert.AreEqual(0, model.GetChangedProperties().Count);
            Assert.AreEqual("Joe", model.FirstName);
            Assert.AreEqual("Bloggs", model.LastName);
        }

        [Test]
        public void TestOriginalValuesAreCorrect()
        {
            var model = new TestTrackedModel(false, false);

            model.BeginEdit();
            model.FirstName = "Joe";
            model.LastName = "Bloggs";

            var values = model.OriginalValues;
            Assert.AreEqual(2, values.Count);
            Assert.AreEqual("Fred", values["FirstName"]);
            Assert.AreEqual("Flintstone", values["LastName"]);
        }

        [Test]
        public void BeginEditCommand_WhenPropertyQueried_ReturnsIClarityCommandInstance()
        {
            var model = new TestTrackedModel(false, false);
            var cmd = model.BeginEditCommand;
            Assert.IsNotNull(cmd);
        }

        [Test]
        public void BeginEditCommand_WhenCanExecuteQueries_ReturnsTrue()
        {
            var model = new TestTrackedModel(false, false);
            var cmd = model.BeginEditCommand;

            Assert.AreEqual(true, cmd.CanExecute(null));
        }

        [Test]
        public void EndEditCommand_WhenPropertyQueried_ReturnsIClarityCommandInstance()
        {
            var model = new TestTrackedModel(false, false);
            var cmd = model.EndEditCommand;
            Assert.IsNotNull(cmd);
        }

        [Test]
        public void EndEditCommand_WhenCanExecuteQueries_ReturnsFalse()
        {
            var model = new TestTrackedModel(false, false);
            var cmd = model.EndEditCommand;

            Assert.AreEqual(false, cmd.CanExecute(null));
        }

        [Test]
        public void CancelEditCommand_WhenPropertyQueried_ReturnsIClarityCommandInstance()
        {
            var model = new TestTrackedModel(false, false);
            var cmd = model.CancelEditCommand;
            Assert.IsNotNull(cmd);
        }

        [Test]
        public void CancelEditCommand_WhenCanExecuteQueries_ReturnsFalse()
        {
            var model = new TestTrackedModel(false, false);
            var cmd = model.CancelEditCommand;

            Assert.AreEqual(false, cmd.CanExecute(null));
        }
    }

    class TestTrackedModel : TrackingViewModel
    {

        public TestTrackedModel(bool observeCollectionChanges, bool beginEdit)
            : base(beginEdit)
        {
            if (observeCollectionChanges) ObserveCollectionChanges();
        }

        private string _firstName = "Fred";
        public virtual string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                SetValue(ref _firstName, value, () => FirstName);
            }
        }

        private string _lastName = "Flintstone";
        public virtual string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                SetValue(ref _lastName, value, () => LastName);
            }
        }

        private ObservableCollection<string> _items = new ObservableCollection<string>();
        public virtual ObservableCollection<string> Items
        {
            get
            {
                return _items;
            }

            set
            {
                SetValue(ref _items, value, () => Items);
            }
        }

        internal void IgnoreFirstname()
        {
            DontTrack(() => FirstName);
        }
    }
}
