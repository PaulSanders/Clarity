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
    public class WinformsCommandBuilder : ICommandBuilder
    {
        public IClarityCommand BuildSimple(Action execute, Func<bool> canExecute)
        {
            return new WinformsSimpleCommand(execute, canExecute);
        }

        public IClarityCommand BuildSimple(Action execute)
        {
            return new WinformsSimpleCommand(execute);
        }

		public IClarityCommand BuildSimpleAsync(Action execute, Func<bool> canExecute)
		{
			return new WinformsSimpleAsyncCommand(execute, canExecute);
		}

		public IClarityCommand BuildSimpleAsync(Action execute)
		{
			return new WinformsSimpleCommand(execute);
		}

		public IClarityCommand BuildDelegate<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            return new WinformsDelegateCommand<T>(execute, canExecute);
        }

        public IClarityCommand BuildDelegate<T>(Action<T> execute)
        {
            return new WinformsDelegateCommand<T>(execute);
        }

		public IClarityCommand BuildDelegateAsync<T>(Action<T> execute, Func<T, bool> canExecute)
		{
			return new WinformsDelegateAsyncCommand<T>(execute, canExecute);
		}

		public IClarityCommand BuildDelegateAsync<T>(Action<T> execute)
		{
			return new WinformsDelegateAsyncCommand<T>(execute);
		}
	}
}
