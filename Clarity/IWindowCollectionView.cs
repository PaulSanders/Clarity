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
using System.Collections.ObjectModel;

namespace Clarity
{
    /// <summary>
    /// Interface to allow the notion of a collection of children and a selected child
    /// </summary>
    /// <typeparam name="T">The type of the children</typeparam>
    public interface IWindowCollectionView<T>
    {
        ObservableCollection<T> Children { get; }
        T SelectedChild { get; set; }
    }
}
