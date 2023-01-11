using Clarity;
using Clarity.Commands;

namespace CustomerBrowser
{
    public class UIClarityCommand : PropertyChangedBase
    {
        private string _text;
        public virtual string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetValue(ref _text, value, () => Text);
            }
        }

        private IClarityCommand _command;
        public virtual IClarityCommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                SetValue(ref _command, value, () => Command);
            }
        }
    }
}
