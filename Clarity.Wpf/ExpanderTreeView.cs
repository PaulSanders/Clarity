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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Clarity.Wpf
{
    public class ExpanderTreeView : TreeView
    {

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new ExpanderTreeViewItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExpanderTreeViewItem;
        }

        public bool ShowExpander
        {
            get { return (bool)GetValue(ShowExpanderProperty); }
            set { SetValue(ShowExpanderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowExpander.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowExpanderProperty =
            DependencyProperty.Register("ShowExpander", typeof(bool), typeof(ExpanderTreeView), new UIPropertyMetadata(true));

    }
}
