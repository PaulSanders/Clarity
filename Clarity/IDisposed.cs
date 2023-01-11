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

namespace Clarity
{
    /// <summary>
    /// Defines a property that indicates if the object is disposed
    /// </summary>
    public interface IDisposed : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        bool IsDisposed { get; }
    }
}
