using Clarity;
using Clarity.Wpf;
using System;

namespace WpfFileExplorer
{
    class ExplorerViewLocator : DefaultViewLocator
    {
        public override object LocateView(Type viewModel, System.Reflection.Assembly usingAssembly)
        {
            if(viewModel == typeof(GetAnswerViewModel))
                return base.LocateView(viewModel, typeof(GetAnswerViewModel).Assembly);

            return base.LocateView(viewModel, this.GetType().Assembly);
        }
    }
}
