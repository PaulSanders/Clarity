using Clarity;
using Clarity.Winforms;
using ExplorerLib.ViewModels;

namespace WinformsFileExplorer
{
    class DefaultBootstrapper : WinformsBootstrapper
    {
        public override IWindow Run()
        {
            var vm = ServiceManager.Default.Resolve<MainViewModel>();

            var win = ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, false, true);
            return win;
        }

        protected override void OnRegisterViewModels()
        {
            Register<GetAnswerViewModel>();
            RegisterSingle<IWindowManager, WinformsWindowManager>();

            //viewmodel logic is contained in the ExplorerLib shared library
            ServiceManager.Default.RegisterTypesFromAssembly(typeof(ViewModel), typeof(ExplorerLib.Entities.Drive).Assembly);
        }

        protected override void OnRegisterDefaultItems()
        {
            base.OnRegisterDefaultItems();

            ServiceManager.Default.Unregister<IViewLocator>();
            Register<IViewLocator, ExplorerViewLocator>();
        }
    }
}
