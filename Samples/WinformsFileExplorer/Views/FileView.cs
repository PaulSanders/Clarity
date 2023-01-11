
using Clarity;
using ExplorerLib.Entities;
using ExplorerLib.Messaging;
using ExplorerLib.ViewModels;
using System.Diagnostics;
using System.Windows.Forms;
namespace WinformsFileExplorer.Views
{
    public partial class FileView : Clarity.Winforms.View
    {
        public FileView()
        {
            InitializeComponent();
        }

        public override void BindViewModel()
        {
            base.BindViewModel();

            ViewModel.OnChangeOf(() => ViewModel.ParentFolder).Execute(RefreshData);

            lv.ItemSelectionChanged += lv_ItemSelectionChanged;
            lv.MouseDoubleClick += lv_MouseDoubleClick;
        }

        void lv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = lv.GetItemAt(e.Location.X, e.Location.Y);
            if (item != null)
            {
                if (item.Tag is Folder)
                {
                    ServiceManager.Default.Resolve<IMessageBus>().Publish<FolderSelectedMessage>(new FolderSelectedMessage() { Folder = item.Tag as Folder });
                }

                if (item.Tag is File)
                {
                    var file=item.Tag as File;
                    file.Start();
                }
            }
        }

        void lv_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {

        }

        private void RefreshData()
        {
            foreach (ListViewItem item in lv.Items)
            {
                if (item.Tag != null)
                {
                    ((PropertyChangedBase)item.Tag).Dispose();
                    item.Tag = null;
                }
            }

            lv.Items.Clear();

            if (ViewModel.ParentFolder != null)
            {
                foreach (var folder in ViewModel.ParentFolder.Folders)
                {
                    var item = new ListViewItem(folder.Name);
                    item.ForeColor = System.Drawing.Color.Blue;
                    
                    item.SubItems.Add("Folder");
                    item.SubItems.Add("");

                    item.Tag = folder;
                    lv.Items.Add(item);
                }

                foreach (var file in ViewModel.ParentFolder.Files)
                {
                    var item = new ListViewItem(file.Name);
                    item.SubItems.Add(file.Extension);
                    item.SubItems.Add(file.FileSizeDescription);
                    item.SubItems.Add(file.DateModified.ToString());

                    file.OnChangeOf(() => file.Name).Execute(() => item.Text = file.Name);
                    file.OnChangeOf(() => file.Extension).Execute(() => item.SubItems[0].Text = file.Extension);
                    file.OnChangeOf(() => file.FileSize).Execute(() => item.SubItems[1].Text = file.FileSizeDescription);
                    file.OnChangeOf(() => file.DateModified).Execute(() => item.SubItems[2].Text = file.DateModified.ToString());

                    item.Tag = file;
                    lv.Items.Add(item);
                }
            }
        }

        public new FileViewModel ViewModel
        {
            get
            {
                return (FileViewModel)base.ViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }
    }
}
