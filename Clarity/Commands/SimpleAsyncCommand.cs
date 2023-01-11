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
using System.Threading.Tasks;

namespace Clarity.Commands
{
    /// <summary>
    /// Provideds a simplified way of executing a command that takes no arguments on a background thread.
    /// </summary>
    public class SimpleAsyncCommand : SimpleCommand
    {
        private volatile bool _isExecuting;

        #region construction
        /// <summary>
        /// Creates a new instance, allowing for a method to be executed
        /// </summary>
        /// <param name="execute">Method to execute</param>
        public SimpleAsyncCommand(Action execute)
            : base(execute)
        {
        }

        /// <summary>
        /// Creates a new instance, allowing for a method to be executed and it's execution to be determined
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="canExecute">Method to test to see if the execute can be performed</param>
        public SimpleAsyncCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
        }
        #endregion

        /// <summary>
        /// Returns true if the command can be executed and it is not already executing
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            if (_isExecuting)
            {
                return false;
            }

            return base.CanExecute(parameter);
        }

        /// <summary>
        /// Executes the command. If the command is already executing then no execution will be performed.
        /// Ensures that the busy state is changed accordingly
        /// </summary>
        public override void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _isExecuting = true;
                IsBusy = true;

                Task.Factory.StartNew(() =>
                {
                    ActionToExecute();
                }).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Logger.LogException(task.Exception);
                    }

                    IsBusy = false;
                    _isExecuting = false;

                    Clarity.Execute.OnUIThread(() => Invalidate());
                });
            }
        }
    }
}
