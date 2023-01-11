using Clarity;
using Clarity.Winforms;
using Clarity.Winforms.Converters;

namespace WinformsEarningsCalculator.Income
{
    public partial class IncomeView : Clarity.Winforms.View
    {
        public IncomeView()
        {
            InitializeComponent();
        }
        
        public override void BindViewModel()
        {
                base.BindViewModel();

                BindItemsControl(cboFrequency, "",
                    this.GetPropertyName(() => ViewModel.FrequencyList),
                    "Income.Frequency", new EnumConverter());

            new ValidationBinder().BindModelErrors(this, ViewModel);
        }

        public new IncomeViewModel ViewModel
        {
            get
            {
                return (IncomeViewModel)base.ViewModel;
            }
        }
    }
}
