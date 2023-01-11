// ****************************************************************************
// <copyright>
// Copyright © Paul Sanders 2014
// </copyright>
// ****************************************************************************
// <author>Paul Sanders</author>
// <project>Clarity</project>
// <web>http://clarity.codeplex.com</web>
// <license>
// See license.txt in this solution
// </license>
// ****************************************************************************
// Based on original code done by Josh Smith
// ****************************************************************************
using System.Threading.Tasks;

namespace Clarity.Wpf
{
    public class TreeViewItemViewModel : PropertyChangedBase
    {
        #region Data
        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();
        private bool _lazyLoadChildren;
        private readonly BindableCollection<TreeViewItemViewModel> _children;
        private TreeViewItemViewModel _parent;

        private bool _isExpanded;
        private bool _isSelected;
        private bool _isChecked;

        private volatile bool _isLoadingChildren;
        private object locker = new object();
        #endregion // Data

        #region Constructors
        private string _name;
        public TreeViewItemViewModel(string name)
        {
            _name = name;
        }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren, string name)
        {
            _name = name;
            _parent = parent;
            _lazyLoadChildren = lazyLoadChildren;

            _children = new BindableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
            {
                _children.Add(DummyChild);
            }
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
        }
        #endregion

        #region Children
        public void ResetChildren()
        {
            bool expand = IsExpanded;
            IsExpanded = false;

            Children.Clear();

            if (_lazyLoadChildren)
                Children.Add(DummyChild);

            NotifyPropertyChanged(() => HasChildren);
            NotifyPropertyChanged(() => HasDummyChild);

            if (expand && Children.Count > 0)
            {
                Children[0].IsExpanded = true;
                IsExpanded = true;
            }
        }

        protected void AddChild(TreeViewItemViewModel child)
        {
            Execute.OnUIThread(() => Children.Add(child));
        }
        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
            NotifyPropertyChanged(() => HasChildren);
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public BindableCollection<TreeViewItemViewModel> Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return Children != null && Children.Count == 1 && Children[0] == DummyChild; }
        }

        public bool HasChildren
        {
            get
            {
                return Children != null && Children.Count > 0;
            }
        }
        #endregion

        public bool IsDummy()
        {
            return this == DummyChild;
        }

        #region Selection / Expansion
        private bool _autoSelectOnExpand;
        public virtual bool AutoSelectOnExpand
        {
            get
            {
                return _autoSelectOnExpand;
            }
            set
            {
                SetValue(ref _autoSelectOnExpand, value, () => AutoSelectOnExpand, () => IsExpanded = IsExpanded);
            }
        }

        private bool _showExpander = true;

        /// <summary>
        /// Show child items in an expander
        /// </summary>
        public bool ShowExpander
        {
            get
            {
                return _showExpander;
            }
            set
            {
                SetValue(ref _showExpander, value, () => ShowExpander);
            }
        }
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                // Expand all the way up to the root.
                if (value && _parent != null)
                    _parent.IsExpanded = true;

                SetValue(ref _isExpanded, value, () => IsExpanded, () =>
                    {
                        // Lazy load the child items, if necessary.
                        if (_isExpanded)
                        {
                            EnsureChildren();
                            if (AutoSelectOnExpand) IsSelected = true;
                        }
                    });
            }
        }

        protected void EnsureChildren()
        {
            // Lazy load the child items, if necessary.
            bool isLoadingChildren;
            lock (locker)
            {
                isLoadingChildren = _isLoadingChildren;
            }

            if (this.HasDummyChild && !isLoadingChildren)
            {
                _isLoadingChildren = true;

                this.LoadChildrenAsync();
            }
        }

        private void LoadChildrenAsync()
        {
            Task.Factory.StartNew(() =>
            {
                LoadChildren();
            }).ContinueWith(task =>
            {
                Execute.OnUIThread(() => _children.Remove(DummyChild));

                _isLoadingChildren = false;
            });
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetValue(ref _isSelected, value, () => IsSelected,() => OnSelected(_isSelected));
            }
        }

        protected virtual void OnSelected(bool isSelected)
        {

        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is checked.
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetValue(ref _isChecked, value, () => IsChecked, () => OnChecked(_isChecked));
            }
        }

        protected virtual void OnChecked(bool isChecked)
        {

        }
        #endregion

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        public string DisplayName
        {
            get
            {
                return _name;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_name))
                return base.ToString();

            return _name;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _parent = null;

            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.Dispose();
                }
                _children.Clear();
            }
        }
    }
}
