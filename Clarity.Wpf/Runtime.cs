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

namespace Clarity.Wpf
{
    public static class Runtime
    {
        public static bool GetDebugOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(DebugOnlyProperty);
        }

        public static void SetDebugOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(DebugOnlyProperty, value);
        }

        // Using a DependencyProperty as the backing store for DebugOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DebugOnlyProperty =
            DependencyProperty.RegisterAttached("DebugOnly", typeof(bool), typeof(Runtime), new UIPropertyMetadata(false, OnDebugOnlyChanged));

        private static void OnDebugOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ui = sender as UIElement;
            if (ui != null)
            {
                if ((bool)e.NewValue)
                {
#if !DEBUG
                ui.Visibility = Visibility.Collapsed;
                ui.IsEnabled = false;
#endif
                }
            }
        }
    }
}
