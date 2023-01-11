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

namespace Clarity.Commands
{
    public class DelegateCommand<T> : PropertyChangedBase, IClarityCommand
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        //keep a local list of handlers, so we can unhook from CommandManager correctly
        private List<EventHandler> _handlers = new List<EventHandler>();

        #region construction
        /// <summary>
        /// Creates a new instance, allowing for a method to be executed
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="function">The <see cref="SecureUseCaseFunction"/> to have check access against</param>
        public DelegateCommand(Action<T> execute)
        {
			execute.IfNullThrow("execute");
            _execute = execute;
        }

        /// <summary>
        /// Creates a new instance, allowing for a method to be executed and it's execution to be determined
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="canExecute">Method to test to see if the execute can be performed</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
			execute.IfNullThrow("execute");
			canExecute.IfNullThrow("canExecute");
			_execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        /// <summary>
        /// Event to help manage command execution
        /// </summary>
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                _handlers.Add(value);
                OnCanExecuteChangedListenerChanged(true, value);
            }

            remove
            {
                _handlers.Remove(value);
                OnCanExecuteChangedListenerChanged(false, value);
            }
        }

        protected virtual void OnCanExecuteChangedListenerChanged(bool listenerAdded, EventHandler handler)
        {

        }

        /// <summary>
        /// Returns true if the method can be executed
        /// </summary>
        /// <param name="parameter">A parameter related to the execution</param>
        /// <returns>True if the command can be executed, otherwise false</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        /// <summary>
        /// Executes the attached method, if allowed to
        /// </summary>
        /// <param name="parameter">The CommandParameter to be passed to the <see cref="Action"/></param>
        public virtual void Execute(object parameter)
        {
            SetBusyStatus(true);
            try
            {
                _execute((T)parameter);
            }
            finally
            {
                SetBusyStatus(false);
            }
        }

        protected Action<T> ActionToExecute
        {
            get
            {
                return _execute;
            }
        }

        protected virtual void Invalidate()
        {
//            CommandManager.InvalidateRequerySuggested();
        }

		protected void SetBusyStatus(bool isBusy)
		{
			IsBusy = isBusy;
		}

		private bool _isBusy;
		public virtual bool IsBusy
		{
			get
			{
				return _isBusy;
			}

			set
			{
				SetValue(ref _isBusy, value, () => IsBusy);
			}
		}

		protected override void OnDispose()
        {
            base.OnDispose();

            var handlers = _handlers.ToArray();
            foreach (var handler in handlers)
            {
                CanExecuteChanged -= handler;
            }

            _execute = null;
            _canExecute = null;
        }
    }
}
