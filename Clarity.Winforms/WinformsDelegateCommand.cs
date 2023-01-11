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
using Clarity.Commands;

namespace Clarity.Winforms
{
    public class WinformsDelegateCommand<T> : DelegateCommand<T>
    {
        public WinformsDelegateCommand(Action<T> execute)
            : base(execute)
        {

        }

        public WinformsDelegateCommand(Action<T> execute, Func<T, bool> canExecute)
            : base(execute, canExecute)
        {

        }

        protected override void OnCanExecuteChangedListenerChanged(bool listenerAdded, EventHandler handler)
        {
            base.OnCanExecuteChangedListenerChanged(listenerAdded, handler);
        }

        protected override void Invalidate()
        {
            base.Invalidate();
        }
    }

	public class WinformsDelegateAsyncCommand<T> : DelegateAsyncCommand<T>
	{
		public WinformsDelegateAsyncCommand(Action<T> execute)
			: base(execute)
		{

		}

		public WinformsDelegateAsyncCommand(Action<T> execute, Func<T, bool> canExecute)
			: base(execute, canExecute)
		{

		}

		protected override void OnCanExecuteChangedListenerChanged(bool listenerAdded, EventHandler handler)
		{
			base.OnCanExecuteChangedListenerChanged(listenerAdded, handler);
		}

		protected override void Invalidate()
		{
			base.Invalidate();
		}
	}
}
