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
    public interface ICommandBuilder
    {
        IClarityCommand BuildSimple(Action execute, Func<bool> canExecute);
        IClarityCommand BuildSimple(Action execute);

		IClarityCommand BuildSimpleAsync(Action execute, Func<bool> canExecute);
		IClarityCommand BuildSimpleAsync(Action execute);

		IClarityCommand BuildDelegate<T>(Action<T> execute, Func<T, bool> canExecute);
        IClarityCommand BuildDelegate<T>(Action<T> execute);

		IClarityCommand BuildDelegateAsync<T>(Action<T> execute, Func<T, bool> canExecute);
		IClarityCommand BuildDelegateAsync<T>(Action<T> execute);
	}
}
