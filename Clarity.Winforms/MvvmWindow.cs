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
using System.Windows.Forms;

namespace Clarity.Winforms
{
    public class MvvmWindow : Form, IWindow, IDisposable
    {
        private IContainer _container;
        private BindingSource _bindingSource;
        private ViewModel _viewModel;
        private IClosable _closablevm;

        public MvvmWindow(ViewModel viewModel)
        {
            _viewModel = viewModel;
            _closablevm = viewModel;
            _closablevm.IsCloseEnabled = true;
            viewModel.OnChangeOf(() => viewModel.IsClosed).Execute(() => Close());

            _container = new Container();
            _bindingSource = new BindingSource(_container);

            ((ISupportInitialize)_bindingSource).BeginInit();

            _bindingSource.DataSource = viewModel;

            DataBindings.Add("Text", ViewModel, "Title");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
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

        public string Title
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }

        public object DataContext
        {
            get
            {
                return _viewModel;
            }
        }

        public ViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public BindingSource BindingSource
        {
            get
            {
                return _bindingSource;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_container != null))
            {
                _container.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
