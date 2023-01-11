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
using System.Reflection;

namespace Clarity
{
    /// <summary>
    /// Provides ability to resolve a view from a ViewModel
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Tries to locate a view based on the provided viewmodel type
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="usingAssembly">The assembly to scan.</param>
        /// <returns>A new instance of the object if found</returns>
        /// <exception cref="Exception">Thrown, if the viewmodel type is</exception>
        object LocateView(Type viewModel, Assembly usingAssembly);

        /// <summary>
        /// Tries to locate a view based on the provided viewmodel type
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="usingAssembly">The assembly to scan.</param>
        /// <returns>A new instance of the object if found</returns>
        /// <exception cref="Exception">Thrown, if the viewmodel type is</exception>
        T LocateView<T>(Type viewModel, Assembly usingAssembly);
    }
}
