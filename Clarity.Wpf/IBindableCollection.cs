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
using System.Collections;
using System.Collections.Generic;

namespace Clarity.Wpf
{
    public interface IBindableCollection
    {
        bool IsNotifying { get; set; }

        void AddRange(IEnumerable items);
        void Refresh();

        void Clear();
    }

    public interface IBindableCollection<T> : IBindableCollection
    {
        void Add(T item);
        void AddRange(IEnumerable<T> items);

        bool Remove(T item);        
        void RemoveRange(IEnumerable<T> items);
    }
}
