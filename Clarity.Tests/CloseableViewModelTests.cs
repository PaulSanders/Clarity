using Clarity.Commands;
using NUnit.Framework;

namespace Clarity.Tests
{
    [TestFixture]
    public class CloseableViewModelTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            ServiceManager.Default.UnregisterAll();
            ServiceManager.Default.Register<ICommandBuilder, DefaultCommandBuilder>();
        }

        [Test]
        public void TestModelCanCloseIsFalseWhenNameIsNotFred()
        {
            var vm = new CloseableViewModelTestClass();
            Assert.IsFalse(vm.CanClose());
        }

        [Test]
        public void TestModelCannotCloseUsUntilNameIsFred()
        {
            var vm = new CloseableViewModelTestClass();
            Assert.IsFalse(vm.CanClose());
            vm.CloseCommand.Execute(null);
            Assert.IsFalse(vm.IsClosed);
        }

        [Test]
        public void TestModelCanWhenNameIsFred()
        {
            var vm = new CloseableViewModelTestClass();
            vm.Name = "Fred";
            Assert.IsTrue(vm.CanClose());
        }

        [Test]
        public void TestModelCloses()
        {
            var vm = new CloseableViewModelTestClass();
            vm.Name = "Fred";
            vm.Close();
            Assert.AreEqual(true, vm.IsClosed);
        }

        [Test]
        public void TestModelOnCloseIsNotCalled()
        {
            var vm = new CloseableViewModelTestClass();
            vm.Name = "Fred";
            vm.StopClose = true;
            vm.CloseCommand.Execute(null);
            Assert.AreEqual(false, vm.IsClosed);
        }

        [Test]
        public void TestModelOnCloseFires()
        {
            var vm = new CloseableViewModelTestClass();
            vm.Name = "Fred";
            vm.CloseCommand.Execute(null);
            Assert.AreEqual(true, vm.IsClosed);
        }

        [Test]
        public void TestModelClosesAfterSettingDisplayResult()
        {
            var vm = new CloseableViewModelTestClass();
            vm.Name = "Fred";
            vm.DisplayResult = true;
            Assert.AreEqual(true, vm.DisplayResult);
            Assert.AreEqual(true, vm.IsClosed);
        }
    }

    class CloseableViewModelTestClass : ViewModel
    {
        private string _name;
        public virtual string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetValue(ref _name, value, () => Name);
            }
        }

        protected override void OnClose()
        {
            if (!StopClose)
            {
                base.OnClose();
            }
        }

        public bool StopClose { get; set; }

        public override bool CanClose()
        {
            return Name == "Fred";
        }
    }
}
