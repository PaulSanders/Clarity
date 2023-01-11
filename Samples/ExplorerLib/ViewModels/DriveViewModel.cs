using Clarity;
using Clarity.Commands;
using ExplorerLib.Entities;
using ExplorerLib.Messaging;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExplorerLib.ViewModels
{
    public class DriveViewModel : ViewModel
    {
        public DriveViewModel()
        {
            SelectedDrive = Drives[0];
        }

        private ObservableCollection<Drive> _drives;
        public virtual ObservableCollection<Drive> Drives
        {
            get
            {
                if (_drives == null)
                {
                    _drives = new ObservableCollection<Drive>();
                    _drives.AddRange(System.IO.DriveInfo.GetDrives().Select(d => new Drive(d)));
                }

                return _drives;
            }
        }

        #region SelectDrive Command
        private IClarityCommand _selectDrive;
        public IClarityCommand SelectDrive
        {
            get
            {
                if (_selectDrive == null)
                {
                    _selectDrive = BuildDelegateCommand<Drive>(ExecuteSelectDrive);
                }
                return _selectDrive;
            }
        }

        private void ExecuteSelectDrive(Drive drive)
        {
            SelectedDrive = drive;
        }

        private Drive _selectedDrive;
        public virtual Drive SelectedDrive
        {
            get
            {
                return _selectedDrive;
            }
            set
            {
                SetValue(ref _selectedDrive, value, () => SelectedDrive, RaiseDriveSelectedMessage);
            }
        }

        private void RaiseDriveSelectedMessage()
        {
            MessageBus.Publish<DriveSelectedMessage>(new DriveSelectedMessage() { Drive = SelectedDrive });
        }
        #endregion

    }
}
