using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Clarity;
using Clarity.Commands;

namespace SearchExample
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel()
        {
            Title = "Get Cities for a Country";

            OnChangeOf(() => SearchText).ExecuteAfterDelay(GetWeather, TimeSpan.FromSeconds(0.8), false);
        }

        private WeatherService.GlobalWeatherSoapClient GetWeatherService()
        {
            return ServiceManager.Default.Resolve<WeatherService.GlobalWeatherSoapClient>();
        }

        private string _searchText;
        public virtual string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                SetValue(ref _searchText, value, () => SearchText);
            }
        }

        private CancellationTokenSource _tokenSource;
        private Task<string> _currentSearch;

        private async void GetWeather()
        {
            var service = GetWeatherService();

            Searching = true;
            try
            {
                if (_currentSearch != null && _tokenSource != null)
                {
                    _tokenSource.Cancel();
                }

                _tokenSource = new CancellationTokenSource();
                _currentSearch = service.GetCitiesByCountryAsync(SearchText);

                var doc = XDocument.Parse(await _currentSearch);
                if (!_tokenSource.Token.IsCancellationRequested)
                {
                    Execute.OnUIThread(() =>
                        {
                            SearchResults.Clear();
                            foreach (var e in doc.Descendants("City"))
                            {
                                var country = e.PreviousNode;

                                SearchResults.Add(new SearchResult() { City = e.Value, Country = ((XElement)country).Value });
                                if (_tokenSource.Token.IsCancellationRequested) break;
                            }

                            if (_tokenSource.Token.IsCancellationRequested)
                            {
                                SearchResults.Clear();
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Error", ex.Message, new OkResult());
            }
            finally
            {
                Searching = false;
            }
        }

        private bool _searching;
        public virtual bool Searching
        {
            get
            {
                return _searching;
            }
            set
            {
                SetValue(ref _searching, value, () => Searching);
            }
        }

        private ObservableCollection<SearchResult> _searchResults = new ObservableCollection<SearchResult>();
        public virtual ObservableCollection<SearchResult> SearchResults
        {
            get
            {
                return _searchResults;
            }
            set
            {
                SetValue(ref _searchResults, value, () => SearchResults);
            }
        }

        private SearchResult _selectedItem;
        public virtual SearchResult SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                SetValue(ref _selectedItem, value, () => SelectedItem);
            }
        }

        #region GetWeatherCommand Command
        private IClarityCommand _getWeatherCommand;
        public IClarityCommand GetWeatherCommand
        {
            get
            {
                if (_getWeatherCommand == null)
                {
                    _getWeatherCommand = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteGetWeather, CanExecuteGetWeather);
                }
                return _getWeatherCommand;
            }
        }

        private bool CanExecuteGetWeather()
        {
            return SelectedItem != null;
        }

        private async void ExecuteGetWeather()
        {
            var service = GetWeatherService();
            var weatherXml = await service.GetWeatherAsync(SelectedItem.City, SelectedItem.Country);
            if (weatherXml.ToLower().Contains("data not found"))
            {
                ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Weather Service", weatherXml, new OkResult());
            }
            else
            {
                var doc = XDocument.Parse(weatherXml);

                var data = new WeatherData();
                foreach (var p in typeof(WeatherData).GetProperties().Where(p => p.CanWrite))
                {
                    var item = doc.Descendants(p.Name).FirstOrDefault();
                    if (item != null)
                    {
                        p.SetValue(data, item.Value);
                    }
                }

                ShowWeather(data);
            }
        }
        #endregion

        private void ShowWeather(WeatherData data)
        {
            var wm = ServiceManager.Default.Resolve<IWindowManager>();
            var vm = ServiceManager.Default.Resolve<WeatherViewModel>();
            vm.Data = data;

            wm.ShowDialog(vm, true, true, false);
        }

    }
}
