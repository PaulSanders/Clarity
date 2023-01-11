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
    public class WinformsSimpleCommand : SimpleCommand
    {
        public WinformsSimpleCommand(Action execute)
            : base(execute)
        {
        }

        public WinformsSimpleCommand(Action execute, Func<bool> canExecute)
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

	public class WinformsSimpleAsyncCommand : SimpleAsyncCommand
	{
		public WinformsSimpleAsyncCommand(Action execute)
			: base(execute)
		{
		}

		public WinformsSimpleAsyncCommand(Action execute, Func<bool> canExecute)
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
