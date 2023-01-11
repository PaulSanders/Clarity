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
using System.Collections.Generic;
using System.Linq;

namespace Clarity
{
    /// <summary>
    /// Provides a mechanism to re-use a window for multiple viewmodels of a given type
    /// </summary>
    /// <typeparam name="TWindowViewModel">The <see cref="ViewModel"/> container</typeparam>
    /// <typeparam name="TChild">The type of child it exposes</typeparam>
    public class SingleWindowManager<TWindowViewModel, TChild> where TWindowViewModel : ViewModel, IWindowCollectionView<TChild>
    {
        public IWindow Window { get; private set; }
        public TWindowViewModel WindowViewModel { get; private set; }

        /// <summary>
        /// Shows a window for the given results. If a window is already open, it will be re-used
        /// </summary>
        /// <param name="items">The items to show</param>
        /// <param name="showWindow">A function to handle the display of the window</param>
        /// <param name="append">If true, appends the items to the existing collection, otherwise the existing items are cleared</param>
        public void Show(IEnumerable<TChild> items, Func<TWindowViewModel, IWindow> showWindow = null, bool append = false)
        {
            if (WindowViewModel == null)
            {
                WindowViewModel = ServiceManager.Default.Resolve<TWindowViewModel>();
                WindowViewModel.OnChangeOf(() => WindowViewModel.IsClosed).Execute(() =>
                {
                    WindowViewModel = null;
                    Window = null;
                });
            }

            if (!append) WindowViewModel.Children.Clear();
            WindowViewModel.Children.AddRange(items);

            if (Window == null)
            {
                if (showWindow == null)
                {
                    Window = ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(WindowViewModel, true, true, false);
                }
                else
                {
                    Window = showWindow(WindowViewModel);
                }
            }

            Window.Show();
        }

        /// <summary>
        /// Shows a single instance window.
        /// </summary>
        /// <param name="predicate">The comparison to perform when determining if a given child should be focused</param>
        /// <param name="createChild">A function to create a child viewmodel</param>
        /// <param name="showWindow">A function to handle the display of the window</param>
        public void Show(Func<TChild, bool> predicate, Func<TChild> createChild, Func<TWindowViewModel, IWindow> showWindow = null)
        {
            if (WindowViewModel == null)
            {
                WindowViewModel = ServiceManager.Default.Resolve<TWindowViewModel>();
                WindowViewModel.OnChangeOf(() => WindowViewModel.IsClosed).Execute(() =>
                {
                    WindowViewModel = null;
                    Window = null;
                });
            }

            if (predicate != null && WindowViewModel.Children.Any(predicate))
            {
                WindowViewModel.SelectedChild = WindowViewModel.Children.First(predicate);
            }
            else
            {
                var vm = createChild();
                if (vm != null)
                {
                    WindowViewModel.Children.Add(vm);
                    WindowViewModel.SelectedChild = vm;
                }
            }

            if (Window == null)
            {
                if (showWindow == null)
                {
                    Window = ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(WindowViewModel, true, true, false);
                }
                else
                {
                    Window = showWindow(WindowViewModel);
                }
            }

            Window.Show();
        }
    }
}
