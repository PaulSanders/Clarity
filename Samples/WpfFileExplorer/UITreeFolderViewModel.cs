using Clarity;
using Clarity.Wpf;
using ExplorerLib.Entities;
using System.Linq;

namespace WpfFileExplorer
{
    public class UITreeFolderViewModel : TreeViewItemViewModel
    {
        private Folder _folder;
        public UITreeFolderViewModel(Folder model)
            : base(null, true, model.Name)
        {
            _folder = model;
        }

        public UITreeFolderViewModel(Folder model, TreeViewItemViewModel parent)
            : base(parent, true, model.Name)
        {
            _folder = model;
        }

        public Folder Folder
        {
            get
            {
                return _folder;
            }
        }

        protected override void LoadChildren()
        {
            _folder.Refresh();
            foreach (var folder in _folder.Folders.OrderBy(f => f.Name))
            {
                AddChild(new UITreeFolderViewModel(folder, this));
            }
        }

        protected override void OnSelected(bool isSelected)
        {
            base.OnSelected(isSelected);
            if (isSelected)
            {
                ServiceManager.Default.Resolve<IMessageBus>().Publish(new ExplorerLib.Messaging.FolderSelectedMessage() { Folder = _folder });
            }
        }
    }
}

