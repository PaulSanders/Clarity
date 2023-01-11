
namespace CustomerBrowser.Messaging
{
    class CustomerAddedMessage
    {
        public Domain.Customer Customer { get; set; }
    }

    class CustomerRemovedMessage
    {
        public Domain.Customer Customer { get; set; }
    }

    class AddNewCustomerMessage
    {
    }

    class EditCustomerMessage
    {
        public Domain.Customer Customer { get; set; }
    }

    class DeleteCustomerMessage
    {
        public Domain.Customer Customer { get; set; }
    }
}
