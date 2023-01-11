using Clarity;

namespace CustomerBrowser.Domain
{
    //some simple entities...
    public abstract class Entity : TrackingViewModel
    {
        public Entity()
        {
            DontTrack(() => Id);
            OnChangeOf(() => Id).Validate(() => IsNew);
        }

        private int _id;
        public virtual int Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetValue(ref _id, value, () => Id);
            }
        }

        public virtual bool IsNew
        {
            get
            {
                return _id == 0;
            }
        }

    }

    public class Customer : Entity
    {
        public Customer()
        {

        }

        public Customer(int id, string firstname, string lastname)
        {
            using (var ctx = CreateInitializationContext())
            {
                Id = id;
                FirstName = firstname;
                LastName = lastname;
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
    }
}
