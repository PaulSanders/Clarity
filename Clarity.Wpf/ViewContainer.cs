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
using System.Windows;
using System.Windows.Controls;

namespace Clarity.Wpf
{
    public class ViewContainer : ContentControl
    {
        public ViewModel ViewModel
        {
            get { return (ViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ViewModel), typeof(ViewContainer), new PropertyMetadata(null, ViewModelChanged));

        private static void ViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = d as ViewContainer;

            if (e.NewValue != null)
            {
                var view = ServiceManager.Default.Resolve<IViewLocator>().LocateView(e.NewValue.GetType(), e.NewValue.GetType().Assembly) as FrameworkElement;

                if (view != null)
                {
                    view.DataContext = e.NewValue;
                    container.Content = view;
                }
            }
        }

        public Type DesignTimeView
        {
            get { return (Type)GetValue(DesignTimeViewProperty); }
            set { SetValue(DesignTimeViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DesignTimeViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DesignTimeViewProperty =
            DependencyProperty.Register("DesignTimeView", typeof(Type), typeof(ViewContainer), new PropertyMetadata(null, OnDesignTimeViewTypeChanged));

        private static void OnDesignTimeViewTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d))
            {
                if (e.NewValue != null)
                {
                    var vc = (ViewContainer)d;
                    var type = vc.DesignTimeView;
                    var view = Activator.CreateInstance(type);

                    vc.Content = view;
                }
            }
        }
    }
}
