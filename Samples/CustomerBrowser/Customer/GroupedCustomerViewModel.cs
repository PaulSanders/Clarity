using Clarity;
using System.Collections.ObjectModel;

namespace CustomerBrowser.Customer
{
    public class GroupedCustomerViewModel : ViewModel, IWindowCollectionView<EditCustomerViewModel>
    {
        public GroupedCustomerViewModel()
        {
            ObserveCollectionChanges();

            Title = "Customers";
        }

        private ObservableCollection<EditCustomerViewModel> _children = new ObservableCollection<EditCustomerViewModel>();
        public ObservableCollection<EditCustomerViewModel> Children
        {
            get
            {
                return _children;
            }
        }

        private EditCustomerViewModel _selectedChild;
        public virtual EditCustomerViewModel SelectedChild
        {
            get
            {
                return _selectedChild;
            }
            set
            {
                SetValue(ref _selectedChild, value, () => SelectedChild);
            }
        }

        protected override void OnCollectionChanged(string propertyName, System.Collections.IList originalItems, System.Collections.IList newItems, System.Collections.IList removedItems)
        {
            base.OnCollectionChanged(propertyName, originalItems, newItems, removedItems);

            if (propertyName == this.GetPropertyName(() => Children))
            {
                foreach (EditCustomerViewModel vm in newItems)
                {
                    if (vm != null)
                    {
                        //when the IsClosed property is set, remove the entry from the collection
                        vm.OnChangeOf(() => vm.IsClosed).Execute(() =>
                        {
                            Children.Remove(vm);
                            if (Children.Count == 0) Close();
                        });
                    }
                }
            }
        }

        public override bool CanClose()
        {
            bool ok = true;
            bool askQuestion = false;

            foreach (var vm in Children)
            {
                if (vm.IsChanged)
                {
                    askQuestion = true;
                    break;
                }
            }

            if (askQuestion) ok = (ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Cancel All Changes", "Are you sure you want to cancel all your changes?", new YesResult(), new NoResult()) == new YesResult());

            if (ok)
            {
                foreach (var vm in Children)
                {
                    vm.Customer.CancelEdit();
                }
            }

            return ok;
        }
    }
}
