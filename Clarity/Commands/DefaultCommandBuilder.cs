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

namespace Clarity.Commands
{
    public class DefaultCommandBuilder : ICommandBuilder
    {
        public IClarityCommand BuildSimple(Action execute, Func<bool> canExecute)
        {
            return new SimpleCommand(execute, canExecute);
        }

        public IClarityCommand BuildSimple(Action execute)
        {
            return new SimpleCommand(execute);
        }

		public IClarityCommand BuildSimpleAsync(Action execute, Func<bool> canExecute)
		{
			return new SimpleAsyncCommand(execute, canExecute);
		}

		public IClarityCommand BuildSimpleAsync(Action execute)
		{
			return new SimpleAsyncCommand(execute);
		}

		public IClarityCommand BuildDelegate<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            return new DelegateCommand<T>(execute, canExecute);
        }

        public IClarityCommand BuildDelegate<T>(Action<T> execute)
        {
            return new DelegateCommand<T>(execute);
        }

		public IClarityCommand BuildDelegateAsync<T>(Action<T> execute, Func<T, bool> canExecute)
		{
			return new DelegateAsyncCommand<T>(execute, canExecute);
		}

		public IClarityCommand BuildDelegateAsync<T>(Action<T> execute)
		{
			return new DelegateAsyncCommand<T>(execute);
		}
	}
}
