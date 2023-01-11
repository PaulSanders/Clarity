using Clarity;
using CustomerBrowser.Customer;

namespace CustomerBrowser.Messaging
{
    /// <summary>
    /// Example message handler to use a group viewModels in a single window. Handles re-activating a tab if an existing viewmodel is already shown
    /// </summary>
    class TabbedCustomerMessageHandler : Disposable, ICustomerMessageHandler
    {
        private SingleWindowManager<GroupedCustomerViewModel, EditCustomerViewModel> _customersManager = new SingleWindowManager<GroupedCustomerViewModel, EditCustomerViewModel>();
        public TabbedCustomerMessageHandler()
        {
            MessageBus.Subscribe<AddNewCustomerMessage>(OnAddNewCustomer);
            MessageBus.Subscribe<EditCustomerMessage>(OnEditCustomer);
        }

        private void OnEditCustomer(EditCustomerMessage msg)
        {
            _customersManager.Show((c) => c.Customer.Id == msg.Customer.Id, () =>
            {
                var vm = ServiceManager.Default.Resolve<EditCustomerViewModel>();
                vm.Customer = msg.Customer;
                return vm;
            });
        }

        private void OnAddNewCustomer(AddNewCustomerMessage msg)
        {
            _customersManager.Show(null, () =>
            {
                var vm = ServiceManager.Default.Resolve<EditCustomerViewModel>();
                vm.Customer = new Domain.Customer();
                return vm;
            });
        }
    }
}
