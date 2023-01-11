using ExplorerLib.ViewModels;
using System.Windows.Forms;

namespace WinformsFileExplorer.Views
{
    public partial class DriveView : Clarity.Winforms.View
    {
        public DriveView()
        {
            InitializeComponent();
        }

        public override void BindViewModel()
        {
            base.BindViewModel();

            if (ViewModel == null) return;

            foreach (var drive in ViewModel.Drives)
            {
                AddButton(drive);
            }
        }

        private int left = 0;
        private void AddButton(ExplorerLib.Entities.Drive drive)
        {
            var btn = new RadioButton();
            btn.Text = drive.Name;
            btn.Checked = drive == ViewModel.SelectedDrive;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Width = 40;
            btn.Height = 20;
            btn.Left = left;
            base.BindButton(btn, ViewModel.SelectDrive, drive);

            pnl.Controls.Add(btn);

            left += btn.Width + 2;
        }

        public new DriveViewModel ViewModel
        {
            get
            {
                return (DriveViewModel)base.ViewModel;
            }
            set
            {
                base.ViewModel = value;
            }
        }
    }
}
