using Clarity;

namespace WinformsEarningsCalculator.Income
{
    public class Income : PropertyChangedBase
    {
        public Income()
        {
            IncomeAmount = 50;
        }

        private decimal _incomeAmount;
        public virtual decimal IncomeAmount
        {
            get
            {
                return _incomeAmount;
            }
            set
            {
                SetValue(ref _incomeAmount, value, () => IncomeAmount);
            }
        }

        private IncomeFrequency _frequency = IncomeFrequency.Hour;
        public virtual IncomeFrequency Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                SetValue(ref _frequency, value, () => Frequency);
            }
        }

        public decimal GetPerSecondRate()
        {
            const int HoursInDay = 8;
            switch (_frequency)
            {
                case IncomeFrequency.Hour:
                    return _incomeAmount / (60 * 60);
                case IncomeFrequency.Day:
                    return _incomeAmount / (HoursInDay * 60 * 60);
                case IncomeFrequency.Week:
                    return _incomeAmount / (5 * HoursInDay * 60 * 60);
                case IncomeFrequency.Month:
                    return (_incomeAmount * 12 / 365) / (HoursInDay * 60 * 60);
                default:
                    return (_incomeAmount / (365 * HoursInDay * 60 * 60));
            }
        }
    }
}
