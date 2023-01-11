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
    public interface IWindow
    {
        void Close();

        string Title { get; set; }

        void Show();

        object DataContext { get; }
    }
}
