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
using System.Windows.Forms;

namespace Clarity.Winforms
{
    public class ClarityContext<T> : ApplicationContext
        where T : WinformsBootstrapper, new()
    {
        public ClarityContext()
        {
            base.MainForm = (Form)new T().Run();
            Execute.InitializeWithCurrentContext();
        }
    }
}
