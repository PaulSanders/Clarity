using Clarity;
using ExplorerLib.Entities;
using ExplorerLib.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExplorerLib.ViewModels
{
    public class DriveStructureViewModel : ViewModel
    {
        public DriveStructureViewModel()
        {
            OnChangeOf(() => SelectedDrive).Execute(RefreshStructure);

            MessageBus.Subscribe<DriveSelectedMessage>(OnDriveSelected);
        }

        private void OnDriveSelected(DriveSelectedMessage msg)
        {
            SelectedDrive = msg.Drive;
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
                SetValue(ref _selectedDrive, value, () => SelectedDrive);
            }
        }

        private Folder _selectedFolder;
        public virtual Folder SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }
            set
            {
                SetValue(ref _selectedFolder, value, () => SelectedFolder);
            }
        }

        private void RefreshStructure()
        {
            _folders.Clear();
            try
            {
                Folders.AddRange(System.IO.Directory.EnumerateDirectories(SelectedDrive.Name).Select(f => new Folder() { Path = f }));
            }
            catch (Exception ex)
            {
                ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Error", ex.Message, new OkResult());
            }
        }

        private ObservableCollection<Folder> _folders = new ObservableCollection<Folder>();
        public virtual ObservableCollection<Folder> Folders
        {
            get
            {
                return _folders;
            }
            set
            {
                SetValue(ref _folders, value, () => Folders);
            }
        }
    }
}
