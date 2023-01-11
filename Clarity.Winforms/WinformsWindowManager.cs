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
using System.Reflection;
using System.Windows.Forms;

namespace Clarity.Winforms
{
    public class WinformsWindowManager : IWindowManager
    {
        private IViewLocator _viewLocator;
        private Assembly _viewAssembly;

        public WinformsWindowManager()
        {
            _viewLocator = ServiceManager.Default.Resolve<IViewLocator>();
        }

        public WinformsWindowManager(Assembly viewAssembly)
        {
            viewAssembly.IfNullThrow("viewAssembly");

            _viewLocator = ServiceManager.Default.Resolve<IViewLocator>();
            _viewAssembly = viewAssembly;
        }

        public IWindow ShowWindow(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized, bool canResize = true)
        {
            var win = OnCreateWindow(viewModel, centreScreen, fitToContent, maximized);
            //if (!canResize) win.ResizeMode = ResizeMode.NoResize;
            if (!canResize) win.FormBorderStyle = FormBorderStyle.FixedDialog;

            win.Show();
            return win;
        }

        public IWindow ShowWindow(ViewModel viewModel, bool centreScreen, bool maximized, double width, double height, bool canResize = true)
        {
            var win = OnCreateWindow(viewModel, centreScreen, false, maximized, width, height);
            //if (!canResize) win.ResizeMode = ResizeMode.NoResize;
            if (!canResize) win.FormBorderStyle = FormBorderStyle.FixedDialog;

            win.Show();
            return win;
        }

        public bool? ShowDialog(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized)
        {
            var win = OnCreateWindow(viewModel, centreScreen, fitToContent, maximized);
            win.FormBorderStyle = FormBorderStyle.FixedDialog;

            win.ShowDialog();
            return viewModel.DisplayResult;
        }

        public bool? ShowDialog(ViewModel viewModel, bool centreScreen, bool maximized, double width, double height)
        {
            var win = OnCreateWindow(viewModel, centreScreen, false, maximized, width, height);
            win.FormBorderStyle = FormBorderStyle.FixedDialog;
            win.ShowDialog();
            return viewModel.DisplayResult;
        }

        public IWindow CreateWindow(ViewModel viewModel)
        {
            return CreateWindowForViewModel(viewModel);
        }

        public WindowResult GetAnswer(string title, string message, params WindowResult[] answers)
        {
            WindowResult result = null;
            Execute.OnUIThread(() =>
            {
                using (var vm = ServiceManager.Default.Resolve<GetAnswerViewModel>())
                {
                    vm.Title = title;
                    vm.Message = message;
                    vm.Answers.AddRange(answers);

                    var win = OnCreateWindow(vm, true, true, false, 350, 175);
                    //                    win.ResizeMode = ResizeMode.NoResize;
                    win.FormBorderStyle = FormBorderStyle.FixedDialog;
                    win.MaximizeBox = false;
                    win.MinimizeBox = false;

                    win.ShowDialog();

                    result = vm.SelectedAnswer;
                }
            });

            return result;
        }

        private MvvmWindow OnCreateWindow(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized, double width = 0.0, double height = 0.0)
        {
            var win = CreateWindowForViewModel(viewModel);
            
            if (centreScreen)
            {
                win.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            }

            if (fitToContent)
            {
                win.AutoSize = true;
                win.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            }

            if (maximized)
            {
                win.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                win.Controls[0].Dock = DockStyle.Fill;
            }

            if (width > 0)
            {
                win.Width = Convert.ToInt32(width);
            }

            if (height > 0)
            {
                win.Height = Convert.ToInt32(height);
            }

            return win;
        }

        private MvvmWindow CreateWindowForViewModel(ViewModel viewModel)
        {
            var win = new MvvmWindow(viewModel);
            var view = GetView(viewModel);

            win.Controls.Add(view);
            view.ViewModel = viewModel;
//            view.BindViewModel();

            return win;
        }

        private View GetView(ViewModel viewModel)
        {
            var viewAssembly = _viewAssembly == null ? viewModel.GetType().Assembly : _viewAssembly;
            var view = (View)_viewLocator.LocateView(viewModel.GetType(), viewAssembly);

            return view;
        }
    }
}
