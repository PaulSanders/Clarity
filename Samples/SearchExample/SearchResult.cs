using Clarity;

namespace SearchExample
{
    public class SearchResult : PropertyChangedBase
    {
        private string _country;
        public virtual string Country
        {
            get
            {
                return _country;
            }
            set
            {
                SetValue(ref _country, value, () => Country);
            }
        }

        private string _city;
        public virtual string City
        {
            get
            {
                return _city;
            }
            set
            {
                SetValue(ref _city, value, () => City);
            }
        }

    }
}
