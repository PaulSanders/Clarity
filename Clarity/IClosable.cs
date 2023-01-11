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
namespace Clarity
{
    /// <summary>
    /// Represents an object that can be closed
    /// </summary>
    public interface IClosable
    {
        /// <summary>
        /// Determines whether this instance can close.
        /// </summary>
        /// <returns></returns>
        bool CanClose();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets a value indicating whether this instance is closed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if closed; otherwise, <c>false</c>.
        /// </value>
        bool IsClosed { get; }

        /// <summary>
        /// Gets/sets the availability of close features
        /// </summary>
        bool IsCloseEnabled { get; set; }
    }
}
