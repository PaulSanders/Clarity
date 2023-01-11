
using Clarity;
using ExplorerLib.ViewModels;
namespace WinformsFileExplorer.Views
{
    public partial class MainView : Clarity.Winforms.View
    {
        public MainView()
        {
            InitializeComponent();
        }

        public override void BindViewModel()
        {
            base.BindViewModel();

            driveStructureView1.ViewModel = ServiceManager.Default.Resolve<DriveStructureViewModel>();
            ViewModel.DriveStructureViewModel = driveStructureView1.ViewModel;

            fileView1.ViewModel = ServiceManager.Default.Resolve<FileViewModel>();
            ViewModel.FileViewModel = fileView1.ViewModel;

            driveView1.ViewModel = ServiceManager.Default.Resolve<DriveViewModel>();
            ViewModel.DriveViewModel = driveView1.ViewModel;
        }

        public new MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)base.ViewModel;
            }
        }
    }
}
