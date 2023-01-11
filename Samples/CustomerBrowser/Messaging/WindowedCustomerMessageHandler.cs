using Clarity;
using CustomerBrowser.Customer;
using System.Linq;

namespace CustomerBrowser.Messaging
{
    /// <summary>
    /// Example of how to use multiple windows to show our viewModels. This will also take into account opening the same window more than once
    /// </summary>
    class WindowedCustomerMessageHandler : Disposable, ICustomerMessageHandler
    {
        public WindowedCustomerMessageHandler()
        {
            MessageBus.Subscribe<AddNewCustomerMessage>(OnAddNewCustomer);
            MessageBus.Subscribe<EditCustomerMessage>(OnEditCustomer);
        }

        private void OnEditCustomer(EditCustomerMessage msg)
        {
            var window = FindWindowForCustomer(msg.Customer);
            if (window == null && msg.Customer.IsEditing)
            {
                msg.Customer.CancelEdit();
            }

            if (window != null)
            {
                window.Show();
            }
            else
            {
                var vm = ServiceManager.Default.Resolve<EditCustomerViewModel>();
                vm.Customer = msg.Customer;

                ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, true, false);
            }
        }

        private IWindow FindWindowForCustomer(Domain.Customer customer)
        {
            foreach (var nativeWindow in App.Current.Windows)
            {
                var window = nativeWindow as IWindow;
                if (window != null && window.DataContext != null && window.DataContext.GetType() == typeof(EditCustomerViewModel))
                {
                    var vm = (EditCustomerViewModel)window.DataContext;
                    if (vm.Customer.Id == customer.Id)
                    {
                        return window;
                    }
                }
                else if (window != null && window.DataContext != null && window.DataContext.GetType() == typeof(GroupedCustomerViewModel))
                {
                    var vm = (GroupedCustomerViewModel)window.DataContext;
                    foreach (var childVM in vm.Children)
                    {
                        if (childVM.Customer.Id == customer.Id)
                        {
                            vm.SelectedChild = childVM;
                            return window;
                        }
                    }
                }
            }

            return null;
        }

        private void OnAddNewCustomer(AddNewCustomerMessage msg)
        {
            var vm = ServiceManager.Default.Resolve<EditCustomerViewModel>();
            vm.Customer = new Domain.Customer();

            ServiceManager.Default.Resolve<IWindowManager>().ShowWindow(vm, true, true, false);
        }
    }
}
