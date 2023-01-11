using Clarity;

namespace SimpleEditor.Domain
{
    public abstract class Entity : PropertyChangedBase
    {
        public Entity()
        {
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
                return Id > 0;
            }
        }

    }
}
