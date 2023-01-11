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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Clarity.Wpf
{
    /// <summary>
    /// 
    /// </summary>
    public class MvvmWindow : Window, IWindow
    {
        private IClosable _closablevm;

        public MvvmWindow(ViewModel viewModel)
        {
            viewModel.IfNullThrow("viewModel");

            DataContext = viewModel;
            _closablevm = viewModel;
            _closablevm.IsCloseEnabled = true;

            var binding = new Binding("Title");
            SetBinding(TitleProperty, binding);

            viewModel.OnChangeOf(() => viewModel.IsClosed).Execute(() => Close());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.IfNullThrow("e");

            if (_closablevm.IsClosed == false)
            {
                //user must have clicked close on title bar
                e.Cancel = !_closablevm.CanClose();
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _closablevm.Close();
            base.OnClosed(e);
        }

        public new void Show()
        {
            try
            {
                base.Show();
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Maximized;
                }

                Activate();
            }
            catch (Exception ex)
            {
                //only log the exception, which can happen if we open and close and open quickly
                Logger.LogException(ex);
            }
        }
    }
}
