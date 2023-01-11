using Clarity;
using Clarity.Wpf;
using ExplorerLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfFileExplorer.Views
{
    /// <summary>
    /// Interaction logic for DriveStructureView.xaml
    /// </summary>
    public partial class DriveStructureView : ExpanderTreeView
    {
        public DriveStructureView()
        {
            InitializeComponent();
            DataContextChanged += DriveStructureView_DataContextChanged;
        }

        void DriveStructureView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.OnChangeOf(() => ViewModel.SelectedDrive).ExecuteAfterDelay(SelectedDriveChanged, TimeSpan.FromMilliseconds(100));
                SelectedDriveChanged();
            }
        }

        private void SelectedDriveChanged()
        {
            var tv = this;
            var items = ViewModel.Folders;

            if (items != null && tv != null)
            {
                var list = items.Select(folder => new UITreeFolderViewModel(folder)).ToList();
                tv.SetValue(DriveStructureView.TreeDataProperty, list);
            }
        }

        public List<UITreeFolderViewModel> TreeData
        {
            get { return (List<UITreeFolderViewModel>)GetValue(TreeDataProperty); }
            set { SetValue(TreeDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Contracts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeDataProperty =
            DependencyProperty.Register("TreeData", typeof(List<UITreeFolderViewModel>), typeof(DriveStructureView), new UIPropertyMetadata(null));


        public DriveStructureViewModel ViewModel
        {
            get
            {
                return (DriveStructureViewModel)DataContext;
            }
        }
    }
}
