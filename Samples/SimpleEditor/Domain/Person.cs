
namespace SimpleEditor.Domain
{
    public class Person : Entity
    {
        //Example of monitoring changes of a couple of properties and re-validating the FullName when they are changed
        //public Person()
        //{
        //    OnChangeOf(() => FirstName, () => LastName).Validate(() => FullName);
        //}

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

        //public virtual string FullName
        //{
        //    get
        //    {
        //        return string.Format("{0} {1}", FirstName, LastName);
        //    }
        //}
    }
}
