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
using System.Windows.Input;
using Clarity.Commands;

namespace Clarity.Wpf
{
    public class WpfSimpleCommand : SimpleCommand, ICommand
    {
        public WpfSimpleCommand(Action execute)
            : base(execute)
        {
        }

        public WpfSimpleCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
        }

        protected override void OnCanExecuteChangedListenerChanged(bool listenerAdded, EventHandler handler)
        {
            base.OnCanExecuteChangedListenerChanged(listenerAdded, handler);

            if (listenerAdded)
                CommandManager.RequerySuggested += handler;
            else
                CommandManager.RequerySuggested -= handler;
        }

        protected override void Invalidate()
        {
            base.Invalidate();
            CommandManager.InvalidateRequerySuggested();
        }
    }

	public class WpfSimpleAsyncCommand : SimpleAsyncCommand, ICommand
	{
		public WpfSimpleAsyncCommand(Action execute)
			: base(execute)
		{
		}

		public WpfSimpleAsyncCommand(Action execute, Func<bool> canExecute)
			: base(execute, canExecute)
		{
		}

		protected override void OnCanExecuteChangedListenerChanged(bool listenerAdded, EventHandler handler)
		{
			base.OnCanExecuteChangedListenerChanged(listenerAdded, handler);

			if (listenerAdded)
				CommandManager.RequerySuggested += handler;
			else
				CommandManager.RequerySuggested -= handler;
		}

		protected override void Invalidate()
		{
			base.Invalidate();
			CommandManager.InvalidateRequerySuggested();
		}
	}
}
