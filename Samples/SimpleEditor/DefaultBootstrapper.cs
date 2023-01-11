using Clarity;
using Clarity.Commands;
using Clarity.Wpf;

namespace SimpleEditor
{
    class DefaultBootstrapper : Bootstrapper
    {
        protected override void OnRegisterViewModels()
        {
            base.OnRegisterViewModels();

            ServiceManager.Default.RegisterSingle<IWindowManager, WindowManager>();
            ServiceManager.Default.RegisterSingle(new MessageHandler());
            ServiceManager.Default.Register<GetAnswerViewModel>();
        }

        protected override void OnRegisterCommandBuilder()
        {
            ServiceManager.Default.RegisterSingle<ICommandBuilder, WpfCommandBuilder>();
        }
    }
}