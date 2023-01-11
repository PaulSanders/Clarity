using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Clarity;

namespace CustomerBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new DefaultBootstrapper();

            var vm = ServiceManager.Default.Resolve<Dashboard.DashboardViewModel>();
            ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, false, 500, 600);
        }
    }
}
