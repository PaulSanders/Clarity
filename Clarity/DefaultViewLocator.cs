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
using System.Linq;
using System.Reflection;

namespace Clarity
{
    /// <summary>
    /// Simple implementation of a View Locator. Based on a viewModel type, tries to locate a corresponding view with the same type name but without the model suffix
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        /// <summary>
        /// Tries to locate a view based on the provided viewmodel type
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="usingAssembly">The assembly to scan.</param>
        /// <returns>
        /// A new instance of the object if found
        /// </returns>
        /// <exception cref="System.Exception">Unable to locate view  + viewName</exception>
        public virtual object LocateView(Type viewModel, Assembly usingAssembly)
        {
            viewModel.IfNullThrow("viewModel");
            usingAssembly.IfNullThrow("usingAssembly");

            var viewName = viewModel.Name;

            if (viewName.EndsWith("Model"))
            {
                viewName = viewName.Replace("Model", string.Empty);
            }

            var viewType = usingAssembly.GetTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.Name == viewName);
            if (viewType == null)
            {
                throw new Exception("Unable to locate view " + viewName);
            }

            return Activator.CreateInstance(viewType);
        }

        /// <summary>
        /// Tries to locate a view based on the provided viewmodel type
        /// </summary>
        /// <param name="viewModelType">The view model.</param>
        /// <param name="usingAssembly">The assembly to scan.</param>
        /// <returns>
        /// A new instance of the object if found
        /// </returns>
        public virtual T LocateView<T>(Type viewmodelType, Assembly usingAssembly)
        {
            return (T)LocateView(viewmodelType, usingAssembly);
        }
    }
}
