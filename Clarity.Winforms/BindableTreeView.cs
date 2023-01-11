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
using System;
using System.Collections;
using System.Windows.Forms;

namespace Clarity.Winforms
{
    public delegate void TreeViewNodeAddedEventHandler(object sender, NodeAddedEventArgs e);
    
    public class NodeAddedEventArgs : EventArgs
    {
        public TreeNode Node { get; internal set; }
        public bool Cancel { get; set; }
    }

    public class BindableTreeView : TreeView
    {
        public event TreeViewNodeAddedEventHandler NodedAdded;

        public BindableTreeView()
        {
            
        }
        
        private string _propertyName;
        private string _displayTextProperty;
        private string _childrenPropertyName;

        private Func<PropertyChangedBase, IEnumerable> _onExpand;

        public void Bind(string propertyName, string displayTextProperty, string childrenProperty,Func<PropertyChangedBase,IEnumerable> onExpand)
        {
            _propertyName = propertyName;
            _displayTextProperty = displayTextProperty;
            _childrenPropertyName = childrenProperty;
            _onExpand = onExpand;

            var items = ParentView.ViewModel.GetProperty(propertyName) as IEnumerable;
            if (items == null) return;

            AddNodes(Nodes, items);
        }

        private void AddNodes(TreeNodeCollection collection, IEnumerable items)
        {
            foreach (var item in items)
            {
                var node = new TreeNode();
                node.Text = item.GetProperty(_displayTextProperty).ToString();
                node.Tag = item;
                node.Nodes.Add("");

                var args = new NodeAddedEventArgs() { Node = node };
                if (this.NodedAdded != null)
                {
                    this.NodedAdded(this, args);
                }

                if(!args.Cancel) collection.Add(node);
            }
        }
        
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);

            foreach (TreeNode node in e.Node.Nodes)
            {
                if (node.Tag != null)
                {
                    ((PropertyChangedBase)node.Tag).Dispose();
                    node.Tag = null;
                }
            }

            e.Node.Nodes.Clear();
            var children = OnGetChildren((PropertyChangedBase)e.Node.Tag);
            AddNodes(e.Node.Nodes, children);
        }

        protected virtual IEnumerable OnGetChildren(PropertyChangedBase model)
        {
            if (_onExpand == null) return null;

            var items = _onExpand(model);

            return items;
        }

        public View ParentView
        {
            get
            {
                var parent = base.Parent;
                while (parent != null && !(parent is View))
                    parent = parent.Parent;

                return parent as View;
            }
        }

        private PropertyChangedBase _selectedModel;
        public virtual PropertyChangedBase SelectedModel
        {
            get
            {
                return _selectedModel;
            }
            set
            {
                _selectedModel = value;                
            }
        }

    }
}
