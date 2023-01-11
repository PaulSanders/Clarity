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
    public static class VisibilityHelper
    {


        public static Visibility GetTrueValue(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(TrueValueProperty);
        }

        public static void SetTrueValue(DependencyObject obj, Visibility value)
        {
            obj.SetValue(TrueValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for TrueValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.RegisterAttached("TrueValue", typeof(Visibility), typeof(VisibilityHelper), new PropertyMetadata(Visibility.Visible));



        public static Visibility GetFalseValue(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(FalseValueProperty);
        }

        public static void SetFalseValue(DependencyObject obj, Visibility value)
        {
            obj.SetValue(FalseValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for FalseValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.RegisterAttached("FalseValue", typeof(Visibility), typeof(VisibilityHelper), new PropertyMetadata(Visibility.Collapsed));

        
        
        public static bool GetIsVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisibleProperty);
        }

        public static void SetIsVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisibleProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(VisibilityHelper), new UIPropertyMetadata(true, OnIsVisibleChanged));

        public static bool GetInverse(DependencyObject obj)
        {
            return (bool)obj.GetValue(InverseProperty);
        }

        public static void SetInverse(DependencyObject obj, bool value)
        {
            obj.SetValue(InverseProperty, value);
        }

        // Using a DependencyProperty as the backing store for Inverse.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InverseProperty =
            DependencyProperty.RegisterAttached("Inverse", typeof(bool), typeof(VisibilityHelper), new UIPropertyMetadata(false, OnIsVisibleChanged));

        private static void OnIsVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisibility(sender as UIElement);
        }

        private static void UpdateVisibility(UIElement fe)
        {
            if (fe != null)
            {
                bool value = (bool)fe.GetValue(VisibilityHelper.IsVisibleProperty);
                if ((bool)fe.GetValue(VisibilityHelper.InverseProperty))
                {
                    fe.Visibility = value ? (Visibility)fe.GetValue(VisibilityHelper.FalseValueProperty) : (Visibility)fe.GetValue(VisibilityHelper.TrueValueProperty);
                }
                else
                {
                    fe.Visibility = value ? (Visibility)fe.GetValue(VisibilityHelper.TrueValueProperty) : (Visibility)fe.GetValue(VisibilityHelper.FalseValueProperty);
                }
            }
        }
    }
}
