using Clarity;
using Clarity.Commands;
using Clarity.Wpf;

namespace SearchExample
{
    class DefaultBootstrapper : Bootstrapper
    {
        protected override void OnRegisterViewModels()
        {
            base.OnRegisterViewModels();

            ServiceManager.Default.RegisterSingle<IWindowManager, WindowManager>();
            ServiceManager.Default.Register<GetAnswerViewModel>();
            ServiceManager.Default.Register<WeatherService.GlobalWeatherSoapClient>(() => new WeatherService.GlobalWeatherSoapClient("GlobalWeatherSoap"));
        }

        protected override void OnRegisterCommandBuilder()
        {
            ServiceManager.Default.RegisterSingle<ICommandBuilder, WpfCommandBuilder>();
        }
    }
}
