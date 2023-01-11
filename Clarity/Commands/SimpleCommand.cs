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
    /// <summary>
    /// Provides a bindable command that takes no parameters
    /// </summary>
    public class SimpleCommand : PropertyChangedBase, IClarityCommand
    {
        private Action _execute;
        private Func<bool> _canExecute;

        //keep a local list of handlers, so we can unhook from CommandManager correctly
        private List<EventHandler> _handlers = new List<EventHandler>();

        #region construction
        /// <summary>
        /// Creates a new instance, allowing for a method to be executed
        /// </summary>
        /// <param name="execute">Method to execute</param>
        public SimpleCommand(Action execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
        }

        /// <summary>
        /// Creates a new instance, allowing for a method to be executed and it's execution to be determined
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="canExecute">Method to test to see if the execute can be performed</param>
        public SimpleCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region canExecute
        /// <summary>
        /// Returns true if the method can be executed
        /// </summary>
        /// <param name="parameter">Ignored</param>
        /// <returns>True if the command can be executed, otherwise false</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

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
//                CommandManager.RequerySuggested += value;
                _handlers.Add(value);
                OnCanExecuteChangedListenerChanged(true, value);
            }

            remove
            {
//                CommandManager.RequerySuggested -= value;
                _handlers.Remove(value);
                OnCanExecuteChangedListenerChanged(false, value);
            }
        }
        #endregion

        protected virtual void OnCanExecuteChangedListenerChanged(bool listenerAdded, EventHandler handler)
        {

        }

        #region execute
        /// <summary>
        /// Executes the attached method, if allowed to
        /// </summary>
        /// <param name="parameter">Ignored</param>
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                IsBusy = true;
                try
                {
                    _execute();
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="Action"/> that this command will execute
        /// </summary>
        protected Action ActionToExecute
        {
            get
            {
                return _execute;
            }
        }
        #endregion

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

        protected virtual void Invalidate()
        {
//            CommandManager.InvalidateRequerySuggested();
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
