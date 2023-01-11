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
    public class WindowManager : IWindowManager
    {
        private IViewLocator _viewLocator;
        public WindowManager()
        {
            _viewLocator = ServiceManager.Default.Resolve<IViewLocator>();
        }

        public IWindow ShowWindow(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized, bool canResize = true)
        {
            var win = OnCreateWindow(viewModel, centreScreen, fitToContent, maximized);
            if (!canResize) win.ResizeMode = ResizeMode.NoResize;

            win.Show();
            return (IWindow)win;
        }

        public IWindow ShowWindow(ViewModel viewModel, bool centreScreen, bool maximized, double width, double height, bool canResize = true)
        {
            var win = OnCreateWindow(viewModel, centreScreen, false, maximized, width, height);
            if (!canResize) win.ResizeMode = ResizeMode.NoResize;

            win.Show();
            return (IWindow)win;
        }

        public bool? ShowDialog(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized)
        {
            var win = OnCreateWindow(viewModel, centreScreen, fitToContent, maximized);
            win.ResizeMode = ResizeMode.NoResize;
            
            win.ShowDialog();
            return viewModel.DisplayResult;
        }

        public bool? ShowDialog(ViewModel viewModel, bool centreScreen, bool maximized, double width, double height)
        {
            var win = OnCreateWindow(viewModel, centreScreen, false, maximized, width, height);
            win.ResizeMode = ResizeMode.NoResize;

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

                        var win = OnCreateWindow(vm, true, true, false);
                        win.ResizeMode = ResizeMode.NoResize;
                        win.WindowStyle = WindowStyle.ToolWindow;

                        win.ShowDialog();

                        result = vm.SelectedAnswer;
                    }
                });

            return result;
        }

        protected virtual Window OnCreateWindow(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized, double width = 0.0, double height = 0.0)
        {
            var win = CreateWindowForViewModel(viewModel);

            if (centreScreen)
            {
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            if (fitToContent)
            {
                win.SizeToContent = SizeToContent.WidthAndHeight;
            }

            if (maximized)
            {
                win.WindowState = WindowState.Maximized;
            }
            
            if (width > 0)
            {
                win.Width = width;
            }

            if (height > 0)
            {
                win.Height = height;
            }

            return win;
        }

        private MvvmWindow CreateWindowForViewModel(ViewModel viewModel)
        {
            var win = new MvvmWindow(viewModel);
            win.Content = GetView(viewModel);
            win.DataContext = viewModel;

            return win;
        }

        protected object GetView(ViewModel viewModel)
        {
            return _viewLocator.LocateView(viewModel.GetType(), viewModel.GetType().Assembly);
        }
    }
}
