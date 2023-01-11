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
    public class ExpanderTreeViewItem : TreeViewItem
    {
        private static TreeViewItem _currentItem = null;
        private static bool registeredHandlers = false;

        public ExpanderTreeViewItem()
        {
            if (!registeredHandlers)
            {
                // Get all Mouse enter/leave events for TreeViewItem.
                EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.MouseEnterEvent, new MouseEventHandler(OnMouseTransition), true);
                EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.MouseLeaveEvent, new MouseEventHandler(OnMouseTransition), true);

                // Listen for the UpdateOverItemEvent on all TreeViewItem's.
                EventManager.RegisterClassHandler(typeof(TreeViewItem), UpdateOverItemEvent, new RoutedEventHandler(OnUpdateOverItem));

                registeredHandlers = true;
            }
        }

        //
        // OnUpdateOverItem:  This method is a listener for the UpdateOverItemEvent.  When it is received,
        // it means that the sender is the closest TreeViewItem to the mouse (closest in the sense of the
        // tree, not geographically).

        static void OnUpdateOverItem(object sender, RoutedEventArgs args)
        {
            // Mark this object as the tree view item over which the mouse
            // is currently positioned.
            _currentItem = sender as TreeViewItem;

            // Tell that item to re-calculate the IsMouseDirectlyOverItem property
            _currentItem.InvalidateProperty(IsMouseDirectlyOverItemProperty);

            // Prevent this event from notifying other tree view items higher in the tree.
            args.Handled = true;
        }

        //
        // OnMouseTransition:  This method is a listener for both the MouseEnter event and
        // the MouseLeave event on TreeViewItems.  It updates the _currentItem, and updates
        // the IsMouseDirectlyOverItem property on the previous TreeViewItem and the new
        // TreeViewItem.

        static void OnMouseTransition(object sender, MouseEventArgs args)
        {
            lock (IsMouseDirectlyOverItemProperty)
            {
                if (_currentItem != null)
                {
                    // Tell the item that previously had the mouse that it no longer does.
                    DependencyObject oldItem = _currentItem;
                    _currentItem = null;
                    oldItem.InvalidateProperty(IsMouseDirectlyOverItemProperty);
                }

                // Get the element that is currently under the mouse.
                IInputElement currentPosition = Mouse.DirectlyOver;

                // See if the mouse is still over something (any element, not just a tree view item).
                if (currentPosition != null)
                {
                    // Yes, the mouse is over something.
                    // Raise an event from that point.  If a TreeViewItem is anywhere above this point
                    // in the tree, it will receive this event and update _currentItem.

                    RoutedEventArgs newItemArgs = new RoutedEventArgs(UpdateOverItemEvent);
                    currentPosition.RaiseEvent(newItemArgs);

                }
            }
        }

        public bool IsMouseDirectlyOverItem
        {
            get { return (bool)GetValue(IsMouseDirectlyOverItemProperty); }
            set { SetValue(IsMouseDirectlyOverItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseDirectlyOverItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMouseDirectlyOverItemProperty =
            DependencyProperty.Register("IsMouseDirectlyOverItem", typeof(bool), typeof(ExpanderTreeViewItem),
            new FrameworkPropertyMetadata(null,
                            new CoerceValueCallback(CalculateIsMouseDirectlyOverItem)));

        private static object CalculateIsMouseDirectlyOverItem(DependencyObject item, object value)
        {
            // This method is called when the IsMouseDirectlyOver property is being calculated
            // for a TreeViewItem. 

            if (item == _currentItem)
                return true;
            else
                return false;
        }

        private static readonly RoutedEvent UpdateOverItemEvent = EventManager.RegisterRoutedEvent("UpdateOverItem",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExpanderTreeViewItem));


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
            DependencyProperty.Register("ShowExpander", typeof(bool), typeof(ExpanderTreeViewItem), new UIPropertyMetadata(true));
    }
}
