using Clarity;

namespace ExplorerLib.Entities
{
    public class Drive : PropertyChangedBase
    {
        public Drive(System.IO.DriveInfo di)
        {
            di.IfNullThrow("di");
            Name = di.Name;
            if (di.IsReady)
                Label = di.VolumeLabel;
            else
                Label = string.Empty;
        }

        private string _name;
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetValue(ref _name, value, () => Name);
            }
        }

        private string _label;
        public virtual string Label
        {
            get
            {
                return _label;
            }
            set
            {
                SetValue(ref _label, value, () => Label);
            }
        }

    }
}
