using Clarity;
using ExplorerLib.Entities;
using ExplorerLib.Messaging;
using ExplorerLib.ViewModels;

namespace WinformsFileExplorer.Views
{
    public partial class DriveStructureView : Clarity.Winforms.View
    {
        public DriveStructureView()
        {
            InitializeComponent();
        }

        public override void BindViewModel()
        {
            base.BindViewModel();

            tv.AfterSelect += (o, e) =>
            {
                var folder = (Folder)e.Node.Tag;
                ViewModel.SelectedFolder = folder;

                if (folder != null)
                {
                    folder.Refresh();
                    ServiceManager.Default.Resolve<IMessageBus>().Publish<FolderSelectedMessage>(new FolderSelectedMessage() { Folder = folder });
                }
            };

            ViewModel.OnChangeOf(() => ViewModel.SelectedDrive).Execute(() =>
                {
                    tv.Nodes.Clear();
                    tv.Nodes.Add("drive", ViewModel.SelectedDrive.Name);

                    tv.Bind("Folders", "Name", "Folders",(f) => 
                        {
                            var folder = (Folder)f;
                            folder.Refresh();

                            return folder.Folders;
                        });
                });            
        }

        public new DriveStructureViewModel ViewModel
        {
            get
            {
                return (DriveStructureViewModel)base.ViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }
    }
}
