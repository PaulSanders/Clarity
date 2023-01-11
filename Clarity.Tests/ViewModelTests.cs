using Clarity.Commands;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace Clarity.Tests
{
    [TestFixture]
    public class ViewModelTests
    {
        [SetUp]
        public void Init()
        {
            ServiceManager.Default.UnregisterAll();
            ServiceManager.Default.Register<ICommandBuilder, DefaultCommandBuilder>();
        }

        [Test]
        public void TestTitleIsSetCorrectly()
        {
            var model = new ViewModelTestClass();
            Assert.AreEqual("Test", model.Title);
        }

        [Test]
        public void TestBuiltCommandIsInitialized()
        {
            var model = new ViewModelTestClass();
            var cmd = model.BuiltCommand;

            Assert.IsNotNull(cmd);

            cmd.Execute(null);
            Assert.AreEqual(true, model.BuiltCommandExecuted);
        }

        [Test]
        public void TestCommandsAreDisposed()
        {
            var model = new ViewModelTestClass();
            var cmd = model.Save;

            model.Dispose();
            Assert.IsTrue(((Disposable)cmd).IsDisposed);
        }

        [Test]
        public void TestChildViewModelsAreDisposed()
        {
            var model = new ViewModelTestClass();

            var cmd = model.Save;
            model.CreateChild();
            var child = model.Child;

            model.Dispose();

            Assert.IsTrue(((Disposable)cmd).IsDisposed);
            Assert.IsTrue(child.IsDisposed);
        }
    }

    class ViewModelTestClass : ViewModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "It's a test...")]
        public ViewModelTestClass()
        {
            Title = "Test";
        }

        #region Save Command
        private IClarityCommand _save;
        public IClarityCommand Save
        {
            get
            {
                if (_save == null)
                {
                    _save = new SimpleCommand(ExecuteSave, CanExecuteSave);
                }

                return _save;
            }
        }

        private bool CanExecuteSave()
        {
            //TODO: Determine if command can be executed
            return false;
        }

        private void ExecuteSave()
        {
        }
        #endregion

        #region BuiltCommand Command
        private IClarityCommand _buildCommand;
        public IClarityCommand BuiltCommand
        {
            get
            {
                if (_buildCommand == null)
                {
                    _buildCommand = BuildSimpleCommand(ExecuteBuiltCommand, CanExecuteBuiltCommand);
                }
                return _buildCommand;
            }
        }

        private bool CanExecuteBuiltCommand()
        {
            return true;
        }

        private void ExecuteBuiltCommand()
        {
            BuiltCommandExecuted = true;
        }

        private bool _builtCommandExecuted;
        public virtual bool BuiltCommandExecuted
        {
            get
            {
                return _builtCommandExecuted;
            }
            set
            {
                SetValue(ref _builtCommandExecuted, value, () => BuiltCommandExecuted);
            }
        }

        #endregion


        public void CreateChild()
        {
            Child = new ViewModelTestClass();
            _children = new ObservableCollection<ViewModelTestClass>();

            for (int i = 0; i < 5; i++)
            {
                Children.Add(new ViewModelTestClass());
            }
        }

        private ViewModelTestClass _child;
        public virtual ViewModelTestClass Child
        {
            get
            {
                return _child;
            }

            set
            {
                SetValue(ref _child, value, () => Child);
            }
        }

        private ObservableCollection<ViewModelTestClass> _children;
        public virtual ObservableCollection<ViewModelTestClass> Children
        {
            get
            {
                return _children;
            }

            set
            {
                SetValue(ref _children, value, () => Children);
            }
        }
    }
}
