using System.Collections.ObjectModel;
using System.Windows.Input;
using Clarity;
using Clarity.Commands;
using CustomerBrowser.Messaging;
using System.ComponentModel.DataAnnotations;

namespace CustomerBrowser.Customer
{
    public class EditCustomerViewModel : ViewModel
    {
        public EditCustomerViewModel()
        {
            Commands.Add(new UIClarityCommand() { Text = "Save", Command = Save });

            OnChangeOf(() => IsCloseEnabled).Execute(() =>
            {
                if (IsCloseEnabled) Commands.Add(new UIClarityCommand() { Text = "Close", Command = CloseCommand });
            });

            OnChangeOf(() => FirstName, () => LastName).Execute(() => IsChanged = true);

            Title = "New Customer";
        }

        private Domain.Customer _customer;
        public virtual Domain.Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                SetValue(ref _customer, value, () => Customer, () =>
                    {
                        if (_customer != null)
                        {
                            if (!Customer.IsNew) Title = "Edit Customer";
                            FirstName = Customer.FirstName;
                            LastName = Customer.LastName;
                            IsChanged = false;
                        }
                    });
            }
        }

        private ObservableCollection<UIClarityCommand> _commands = new ObservableCollection<UIClarityCommand>();
        public virtual ObservableCollection<UIClarityCommand> Commands
        {
            get
            {
                return _commands;
            }
            set
            {
                SetValue(ref _commands, value, () => Commands);
            }
        }

        #region Save Command
        private IClarityCommand _save;
        public IClarityCommand Save
        {
            get
            {
                if (_save == null)
                {
                    _save = BuildSimpleCommand(ExecuteSave, CanExecuteSave);
                }
                return _save;
            }
        }

        private bool CanExecuteSave()
        {
            return Customer.IsValid();
        }

        private void ExecuteSave()
        {
            Customer.BeginEdit();
            Customer.FirstName = FirstName;
            Customer.LastName = LastName;
            Customer.EndEdit();

            if (Customer.IsNew)
            {
                MessageBus.Publish(new CustomerAddedMessage() { Customer = Customer });
            }
            
            this.DisplayResult = true;
        }
        #endregion

        public override bool CanClose()
        {
            bool ok = true;

            if (IsChanged)
            {
                ok = (ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Cancel Changes", "Are you sure you want to cancel your changes?",
                    new YesResult(), new NoResult()) == new YesResult());
            }

            return ok;
        }

        private string _firstName;
        [Required]
        public virtual string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                SetValue(ref _firstName, value, () => FirstName);
            }
        }

        private string _lastName;
        [Required]
        public virtual string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                SetValue(ref _lastName, value, () => LastName);
            }
        }

        private bool _isChanged;
        public virtual bool IsChanged
        {
            get
            {
                return _isChanged;
            }
            set
            {
                SetValue(ref _isChanged, value, () => IsChanged);
            }
        }

    }

}
