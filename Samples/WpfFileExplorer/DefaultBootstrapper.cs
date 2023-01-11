using Clarity;
using Clarity.Commands;
using Clarity.Wpf;

namespace WpfFileExplorer
{
    class DefaultBootstrapper : Bootstrapper
    {
        protected override void OnRegisterViewModels()
        {
            Register<GetAnswerViewModel>();
            RegisterSingle<IWindowManager, WindowManager>();

            //viewmodel logic is contained in the ExplorerLib shared library
            ServiceManager.Default.RegisterTypesFromAssembly(typeof(ViewModel), typeof(ExplorerLib.Entities.Drive).Assembly);
        }

        protected override void OnRegisterCommandBuilder()
        {
            ServiceManager.Default.RegisterSingle<ICommandBuilder, WpfCommandBuilder>();
        }

        protected override void OnRegisterDefaultItems()
        {
            base.OnRegisterDefaultItems();

            ServiceManager.Default.Unregister<IViewLocator>();
            Register<IViewLocator, ExplorerViewLocator>();
        }
    }
}
