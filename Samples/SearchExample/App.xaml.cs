using System.Windows;
using Clarity;

namespace SearchExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new DefaultBootstrapper();

            var vm = ServiceManager.Default.Resolve<MainViewModel>();

            ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, false, 400, 400);
        }
    }
}
