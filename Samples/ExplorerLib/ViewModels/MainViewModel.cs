using Clarity;

namespace ExplorerLib.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel()
        {
            Title = "Clarity Explorer";
        }

        private DriveViewModel _driveViewModel;
        public virtual DriveViewModel DriveViewModel
        {
            get
            {
                return _driveViewModel;
            }
            set
            {
                SetValue(ref _driveViewModel, value, () => DriveViewModel);
            }
        }

        private DriveStructureViewModel _driveStructureViewModel;
        public virtual DriveStructureViewModel DriveStructureViewModel
        {
            get
            {
                return _driveStructureViewModel;
            }
            set
            {
                SetValue(ref _driveStructureViewModel, value, () => DriveStructureViewModel);
            }
        }

        private FileViewModel _fileViewModel;
        public virtual FileViewModel FileViewModel
        {
            get
            {
                return _fileViewModel;
            }
            set
            {
                SetValue(ref _fileViewModel, value, () => FileViewModel);
            }
        }

    }
}
