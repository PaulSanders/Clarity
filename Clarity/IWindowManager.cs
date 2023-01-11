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
    /// Defines a basic window construction mechanism to be used by an application
    /// </summary>
    public interface IWindowManager
    {
        IWindow ShowWindow(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized, bool canResize = true);
        IWindow ShowWindow(ViewModel viewModel, bool centreScreen, bool maximized, double width, double height, bool canResize = true);
        IWindow CreateWindow(ViewModel viewModel);

        bool? ShowDialog(ViewModel viewModel, bool centreScreen, bool fitToContent, bool maximized);
        bool? ShowDialog(ViewModel viewModel, bool centreScreen, bool maximized, double width, double height);

        WindowResult GetAnswer(string title, string message, params WindowResult[] answers);
    }
}
