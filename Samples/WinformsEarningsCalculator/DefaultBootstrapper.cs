using Clarity;
using Clarity.Winforms;

namespace WinformsEarningsCalculator
{
    class DefaultBootstrapper : WinformsBootstrapper
    {
        public override IWindow Run()
        {
            var vm = new Income.IncomeViewModel();
            var win = ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, true, false);
            return win;
        }
    }
}
