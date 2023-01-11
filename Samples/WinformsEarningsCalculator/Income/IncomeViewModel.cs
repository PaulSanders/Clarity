using Clarity;
using Clarity.Commands;
using System;
using System.Timers;

namespace WinformsEarningsCalculator.Income
{
    public class IncomeViewModel : ViewModel
    {
        private Timer _timer;

        public IncomeViewModel()
        {
            Income = new Income();
            Income.OnChangeOf<object>(() => Income.IncomeAmount, () => Income.Frequency).Execute(OnIncomeDataChanged);
            OnChangeOf(() => SelectedFrequency).Execute(UpdateIncomeFrequency);

            StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);

            StartTimer();

            Title = "Income Today - Binding Example";
        }

        private void OnIncomeDataChanged()
        {
            NotifyPropertyChanged(() => EarningsToday);
        }

        private Income _income;
        public virtual Income Income
        {
            get
            {
                return _income;
            }
            set
            {
                SetValue(ref _income, value, () => Income);
            }
        }

        private DateTime _startTime;
        public virtual DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                SetValue(ref _startTime, value, () => StartTime);
            }
        }

        public decimal EarningsToday
        {
            get
            {
                return Income.GetPerSecondRate() * Convert.ToDecimal((DateTime.Now - StartTime).TotalSeconds);
            }
        }

        public string[] FrequencyList
        {
            get
            {
                return Enum.GetNames(typeof(IncomeFrequency));
            }
        }

        private string _selectedFrequency = "Hour";
        public virtual string SelectedFrequency
        {
            get
            {
                return _selectedFrequency;
            }
            set
            {
                SetValue(ref _selectedFrequency, value, () => SelectedFrequency, UpdateIncomeFrequency);
            }
        }

        private void UpdateIncomeFrequency()
        {
            Income.Frequency = (IncomeFrequency)Enum.Parse(typeof(IncomeFrequency), _selectedFrequency);
        }

        #region ResetFrequency Command
        private IClarityCommand _resetFrequency;
        public IClarityCommand ResetFrequency
        {
            get
            {
                if (_resetFrequency == null)
                {
                    _resetFrequency = BuildSimpleCommand(ExecuteResetFrequency);
                }
                return _resetFrequency;
            }
        }

        private void ExecuteResetFrequency()
        {
            SelectedFrequency = "Hour";
        }
        #endregion


        #region Test Command
        private IClarityCommand _test;
        public IClarityCommand Test
        {
            get
            {
                if (_test == null)
                {
                    _test = BuildSimpleCommand(ExecuteTest);
                }
                return _test;
            }
        }

        private void ExecuteTest()
        {
            var result = ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Some Question", "This is the message", new YesResult(), new NoResult());
            if (result != null) Answer = result.Text;
        }
        #endregion

        private string _answer;
        public virtual string Answer
        {
            get
            {
                return _answer;
            }
            set
            {
                SetValue(ref _answer, value, () => Answer);
            }
        }

        private void StartTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NotifyPropertyChanged(() => EarningsToday);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _timer.Stop();
            _timer.Elapsed -= _timer_Elapsed;
        }

        private bool _mkeInvalid;
        public virtual bool MakeInvalid
        {
            get
            {
                return _mkeInvalid;
            }
            set
            {
                SetValue(ref _mkeInvalid, value, () => MakeInvalid);
            }
        }

        public override ValidationResults OnValidate(string propertyName)
        {
            var results = base.OnValidate(propertyName);

            if (MakeInvalid && Income.Frequency == IncomeFrequency.Day)
            {
                results.Add("Frequency is invalid", () => Income.Frequency);
            }

            return results;
        }
    }
}