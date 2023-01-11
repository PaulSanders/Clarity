using Clarity;
using Clarity.Commands;
using Clarity.Wpf;
using CustomerBrowser.Domain;

namespace CustomerBrowser
{
    class DefaultBootstrapper : Bootstrapper
    {
        protected override void OnRegisterViewModels()
        {
            base.OnRegisterViewModels();

            ServiceManager.Default.Register<GetAnswerViewModel>();
        }

        protected override void OnRegisterDefaultItems()
        {
            base.OnRegisterDefaultItems();

            ServiceManager.Default.RegisterSingle<IWindowManager, WindowManager>();
            ServiceManager.Default.RegisterSingle(new CustomerDb());
        }

        protected override void OnRegisterCommandBuilder()
        {
            ServiceManager.Default.RegisterSingle<ICommandBuilder, WpfCommandBuilder>();
        }
    }
}
