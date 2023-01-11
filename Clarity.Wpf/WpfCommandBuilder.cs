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

namespace Clarity.Wpf
{
    public class WpfCommandBuilder : ICommandBuilder
    {
        public IClarityCommand BuildSimple(Action execute, Func<bool> canExecute)
        {
            return new WpfSimpleCommand(execute, canExecute);
        }

        public IClarityCommand BuildSimple(Action execute)
        {
            return new WpfSimpleCommand(execute);
        }

		public IClarityCommand BuildSimpleAsync(Action execute, Func<bool> canExecute)
		{
			return new WpfSimpleAsyncCommand(execute, canExecute);
		}

		public IClarityCommand BuildSimpleAsync(Action execute)
		{
			return new WpfSimpleAsyncCommand(execute);
		}

		public IClarityCommand BuildDelegate<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            return new WpfDelegateCommand<T>(execute, canExecute);
        }

        public IClarityCommand BuildDelegate<T>(Action<T> execute)
        {
            return new WpfDelegateCommand<T>(execute);
        }

		public IClarityCommand BuildDelegateAsync<T>(Action<T> execute, Func<T, bool> canExecute)
		{
			return new WpfDelegateAsyncCommand<T>(execute, canExecute);
		}

		public IClarityCommand BuildDelegateAsync<T>(Action<T> execute)
		{
			return new WpfDelegateAsyncCommand<T>(execute);
		}
	}
}
