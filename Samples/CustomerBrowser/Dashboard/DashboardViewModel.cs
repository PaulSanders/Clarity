using System.Collections.ObjectModel;
using System.Windows.Input;
using Clarity;
using Clarity.Commands;
using CustomerBrowser.Domain;
using CustomerBrowser.Messaging;

namespace CustomerBrowser.Dashboard
{
    public class DashboardViewModel : ViewModel
    {
        public DashboardViewModel(CustomerDb db)
        {
            Title = "Customer Browser";

            MessageBus.Subscribe<CustomerAddedMessage>(OnCustomerAdded);
            MessageBus.Subscribe<DeleteCustomerMessage>(OnCustomerDeleted);

            Customers.AddRange(db.Customers);

            SetEditor();
        }

        private void OnCustomerDeleted(DeleteCustomerMessage obj)
        {
            Customers.Remove(obj.Customer);
        }

        private void OnCustomerAdded(CustomerAddedMessage msg)
        {
            Customers.Add(msg.Customer);
        }

        private ObservableCollection<Domain.Customer> _customers = new ObservableCollection<Domain.Customer>();
        public virtual ObservableCollection<Domain.Customer> Customers
        {
            get
            {
                return _customers;
            }
            set
            {
                SetValue(ref _customers, value, () => Customers);
            }
        }

        private Domain.Customer _selectedCustomer;
        public virtual Domain.Customer SelectedCustomer
        {
            get
            {
                return _selectedCustomer;
            }
            set
            {
                SetValue(ref _selectedCustomer, value, () => SelectedCustomer);
            }
        }

        private string _editTypeText;
        public virtual string EditTypeText
        {
            get
            {
                return _editTypeText;
            }
            set
            {
                SetValue(ref _editTypeText, value, () => EditTypeText);
            }
        }

        private bool _editCustomersInNewWindow;
        public virtual bool EditCustomersInNewWindow
        {
            get
            {
                return _editCustomersInNewWindow;
            }
            set
            {
                SetValue(ref _editCustomersInNewWindow, value, () => EditCustomersInNewWindow, () => SetEditor());
            }
        }

        private void SetEditor()
        {
            //unhook the existing message handler if one is registered
            if (ServiceManager.Default.IsRegistered<ICustomerMessageHandler>())
            {
                var handler = ServiceManager.Default.Resolve<ICustomerMessageHandler>();
                handler.Dispose();
                ServiceManager.Default.Unregister<ICustomerMessageHandler>();
            }

            //hook up the new message handler
            var editMode = "Windowed";
            if (_editCustomersInNewWindow)
            {
                ServiceManager.Default.RegisterSingle<ICustomerMessageHandler>(new WindowedCustomerMessageHandler());
            }
            else
            {
                ServiceManager.Default.RegisterSingle<ICustomerMessageHandler>(new TabbedCustomerMessageHandler());
                editMode = "Tabbed";
            }

            EditTypeText = $"Item View Mode: {editMode} [Click to Change]";
        }

        #region AddCustomer Command
        private IClarityCommand _addCustomer;
        public IClarityCommand AddCustomer
        {
            get
            {
                if (_addCustomer == null)
                {
                    _addCustomer = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteAddCustomer);
                }
                return _addCustomer;
            }
        }

        private void ExecuteAddCustomer()
        {
            MessageBus.Publish(new AddNewCustomerMessage());
        }
        #endregion

        #region EditCustomer Command
        private IClarityCommand _editCustomer;
        public IClarityCommand EditCustomer
        {
            get
            {
                if (_editCustomer == null)
                {
                    _editCustomer = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteEditCustomer, CanExecuteEditCustomer);
                }
                return _editCustomer;
            }
        }

        private bool CanExecuteEditCustomer()
        {
            return SelectedCustomer != null;
        }

        private void ExecuteEditCustomer()
        {
            MessageBus.Publish(new EditCustomerMessage() { Customer = SelectedCustomer });
        }
        #endregion

        #region DeleteCustomer Command
        private IClarityCommand _deleteCustomer;
        public IClarityCommand DeleteCustomer
        {
            get
            {
                if (_deleteCustomer == null)
                {
                    _deleteCustomer = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteDeleteCustomer, CanExecuteDeleteCustomer);
                }
                return _deleteCustomer;
            }
        }

        private bool CanExecuteDeleteCustomer()
        {
            return SelectedCustomer != null;
        }

        private void ExecuteDeleteCustomer()
        {
            if (ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Delete this customer?", "Are you sure you want to delete this customer?",
                new YesResult(), new NoResult()) == new YesResult())
            {
                MessageBus.Publish(new DeleteCustomerMessage() { Customer = SelectedCustomer });
            }
        }
        #endregion
    }
}
