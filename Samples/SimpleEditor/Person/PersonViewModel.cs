using Clarity;

namespace SimpleEditor.Person
{
    public class PersonViewModel : TrackingViewModel
    {
        public PersonViewModel()
        {
            Title = "Edit";
            
            DontTrack(() => Person);

            OnChangeOf(() => Person).Execute(() =>
                {
                    if (Person != null)
                    {
                        using (var ctx = base.CreateInitializationContext())
                        {
                            FirstName = Person.FirstName;
                            LastName = Person.LastName;
                        }
                        BeginEdit();
                    }
                });

            OnChangeOf(() => FirstName, () => LastName).Validate(() => FullName);
        }

        private Domain.Person _person;
        public virtual Domain.Person Person
        {
            get
            {
                return _person;
            }
            set
            {
                SetValue(ref _person, value, () => Person);
            }
        }

        private string _firstName;
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

        public virtual string FullName
        {
            get
            {
                if (Person == null) return string.Empty;

                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        
        public override bool CanClose()
        {
            bool ok = true;

            if (IsEditing)
            {
                var result = ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Close", "Closing will lose your changes. Are you sure you want to do that?", new YesResult(), new NoResult());
                if (result == new YesResult())
                {
                    CancelEdit();
                }
                else
                {
                    ok = false;
                }
            }

            return ok;
        }

        protected override bool OnEndEdit()
        {
            Person.FirstName = FirstName;
            Person.LastName = LastName;

            return base.OnEndEdit();
        }
    }
}
