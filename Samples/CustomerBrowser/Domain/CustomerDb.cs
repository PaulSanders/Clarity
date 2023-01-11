using System.Collections.Generic;
using Clarity;
using CustomerBrowser.Messaging;

namespace CustomerBrowser.Domain
{
    //pretend database for our customers
    public class CustomerDb : Disposable
    {
        public CustomerDb()
        {
            _customers = new List<Customer>();
            AddDummmyData();

            MessageBus.Subscribe<CustomerAddedMessage>(OnCustomerAdded);
            MessageBus.Subscribe<CustomerRemovedMessage>(OnCustomerRemoved);
        }

        private void OnCustomerRemoved(CustomerRemovedMessage obj)
        {
            Customers.Remove(obj.Customer);
        }

        private void OnCustomerAdded(CustomerAddedMessage obj)
        {
            Customers.Add(obj.Customer);
            obj.Customer.Id = Customers.Count;
        }

        private List<Customer> _customers;
        public virtual List<Customer> Customers
        {
            get
            {
                return _customers;
            }
            set
            {
                _customers = value;
            }
        }

        private void AddDummmyData()
        {
            _customers.Add(new Customer(1, "Fred", "Flintstone"));
            _customers.Add(new Customer(2, "Wilma", "Flintstone"));
            _customers.Add(new Customer(3, "Barney", "Rubble"));
            _customers.Add(new Customer(4, "Bamm-Bamm", "Flintstone"));
        }
    }
}
