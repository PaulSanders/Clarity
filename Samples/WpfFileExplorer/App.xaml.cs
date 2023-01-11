using Clarity;
using ExplorerLib.ViewModels;
using System.Windows;

namespace WpfFileExplorer
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
            vm.DriveStructureViewModel = ServiceManager.Default.Resolve<DriveStructureViewModel>();
            vm.DriveViewModel = ServiceManager.Default.Resolve<DriveViewModel>();
            vm.FileViewModel = ServiceManager.Default.Resolve<FileViewModel>();

            ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, false, true);
        }
    }
}
