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
using Clarity.Commands;

namespace Clarity.Winforms
{
    public abstract class WinformsBootstrapper : Bootstrapper
    {
        public virtual IWindow Run()
        {
            return null;
        }

        protected override void OnRegisterViewModels()
        {
            base.OnRegisterViewModels();

            Register<GetAnswerViewModel>();
            RegisterSingle<IWindowManager, WinformsWindowManager>();
        }

        protected override void OnRegisterCommandBuilder()
        {
            RegisterSingle<ICommandBuilder, WinformsCommandBuilder>();
        }
    }
}
