using Clarity;
using Clarity.Commands;
using ExplorerLib.Entities;
using ExplorerLib.Messaging;

namespace ExplorerLib.ViewModels
{
    public class FileViewModel : ViewModel
    {
        public FileViewModel()
        {
            MessageBus.Subscribe<ExplorerLib.Messaging.FolderSelectedMessage>(OnFolderSelected);
        }

        private void OnFolderSelected(ExplorerLib.Messaging.FolderSelectedMessage msg)
        {
            msg.Folder.Refresh();
            ParentFolder = msg.Folder;
        }

        private Folder _parentFolder;
        public virtual Folder ParentFolder
        {
            get
            {
                return _parentFolder;
            }
            set
            {
                SetValue(ref _parentFolder, value, () => ParentFolder);
            }
        }

        #region SelectItem Command
        private IClarityCommand _selectItem;
        public IClarityCommand SelectItem
        {
            get
            {
                if (_selectItem == null)
                {
                    _selectItem = BuildDelegateCommand<PropertyChangedBase>(ExecuteSelectItem);
                }
                return _selectItem;
            }
        }

        private void ExecuteSelectItem(PropertyChangedBase item)
        {
            if (item is Folder)
            {
                MessageBus.Publish(new FolderSelectedMessage() { Folder = (Folder)item });
            }
            else if (item is File)
            {
                ((File)item).Start();
            }
        }
        #endregion


    }
}
