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
using System.Windows;
using System.Windows.Data;

namespace Clarity.Wpf
{
    public class CBindingExtension : Binding
    {
        public CBindingExtension()
        {
            Init();
        }

        public CBindingExtension(string path)
        {
            Init();
            Path = new PropertyPath(path);
        }

        public CBindingExtension(object source, string path)
        {
            Init();
            Source = source;
            Path = new PropertyPath(path);
        }

        private void Init()
        {
            Converter = new ClarityCommandConverter();
        }
    }
}
