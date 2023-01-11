using Clarity;

namespace SearchExample
{
    public class WeatherViewModel : ViewModel
    {
        public WeatherViewModel()
        {
            Title = "Weather Details";
        }

        private WeatherData _data;
        public virtual WeatherData Data
        {
            get
            {
                return _data;
            }
            set
            {
                SetValue(ref _data, value, () => Data);
            }
        }

    }
}
