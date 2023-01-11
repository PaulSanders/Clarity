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
using System.Windows.Threading;

namespace Clarity.Wpf
{
    public static class AnimationEx
    {
        public static bool GetFadeIn(DependencyObject obj)
        {
            return (bool)obj.GetValue(FadeInProperty);
        }

        public static void SetFadeIn(DependencyObject obj, bool value)
        {
            obj.SetValue(FadeInProperty, value);
        }

        // Using a DependencyProperty as the backing store for FadeIn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FadeInProperty = DependencyProperty.RegisterAttached("FadeIn", typeof(bool), typeof(AnimationEx), new PropertyMetadata(false, OnFadeInChanged));

        private static void OnFadeInChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var ui = sender as FrameworkElement;

                var animate = new Action(() =>
                {
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(5); //20 frames/sec
                    timer.Tick += (o, args) =>
                        {
                            ui.Opacity += .05;
                            if (ui.Opacity >= 1)
                            {
                                timer.Stop();
                                timer = null;
                            }
                        };
                    timer.Start();
                });

                if (ui.IsLoaded)
                {
                    animate();
                }
                else
                {
                    ui.Loaded += (o, args) => animate();
                }
            }
        }
    }
}
