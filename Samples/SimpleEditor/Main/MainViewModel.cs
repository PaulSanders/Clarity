using System.Collections.ObjectModel;
using System.Windows.Input;
using Clarity;
using Clarity.Commands;

namespace SimpleEditor.Main
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel()
        {
            ObserveCollectionChanges();

            Title = "Simple Person Editor";
        }

        private ObservableCollection<Domain.Person> _people = new ObservableCollection<Domain.Person>();
        public virtual ObservableCollection<Domain.Person> People
        {
            get
            {
                return _people;
            }
            set
            {
                SetValue(ref _people, value, () => People);
            }
        }

        private Domain.Person _selectedPerson;
        public virtual Domain.Person SelectedPerson
        {
            get
            {
                return _selectedPerson;
            }
            set
            {
                SetValue(ref _selectedPerson, value, () => SelectedPerson);
            }
        }

        #region EditPerson Command
        private IClarityCommand _editPerson;
        public IClarityCommand EditPerson
        {
            get
            {
                if (_editPerson == null)
                {
                    _editPerson = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteEditPerson, CanExecuteEditPerson);
                }
                return _editPerson;
            }
        }

        private bool CanExecuteEditPerson()
        {
            return SelectedPerson != null;
        }

        private void ExecuteEditPerson()
        {
            MessageBus.Publish(new EditPersonMessage() { Person = SelectedPerson });
        }
        #endregion
    }
}
