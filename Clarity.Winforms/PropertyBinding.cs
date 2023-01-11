// ****************************************************************************
// <copyright>
// Copyright © Paul Sanders 2014
// </copyright>
// ****************************************************************************
// <author>Paul Sanders</author>
// <project>Clarity</project>
// <web>http://clarity.codeplex.com</web>
// <license>
// See license.txt in this solution
// </license>
// ****************************************************************************
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Clarity.Winforms
{
    [ProvideProperty("PropertyName", typeof(Control))]
    [ProvideProperty("DisplayFormat", typeof(Control))]
    public class PropertyBinding : Component, IExtenderProvider, ISupportInitialize
    {
        private Dictionary<Control, PropertyInfo> _paths = new Dictionary<Control, PropertyInfo>();

        public PropertyBinding()
        {
        }

        public PropertyBinding(IContainer cont)
        {
            cont.Add(this);
        }

        public bool CanExtend(object extendee)
        {
            return extendee is Control && !(extendee is Button);
        }

        [Category("Data")]
        [Description("Set to the name of the property on the ViewModel to bind to.")]
        public string GetPropertyName(Control ctrl)
        {
            return EnsurePropertiesExists(ctrl).ViewModelProperty;
        }

        public void SetPropertyName(Control ctrl, string path)
        {
            EnsurePropertiesExists(ctrl).ViewModelProperty = path;
        }

        [Category("Appearance")]
        [Description("Set how the value should be formatted")]
        public string GetDisplayFormat(Control ctrl)
        {
            return EnsurePropertiesExists(ctrl).Format;
        }

        public void SetDisplayFormat(Control ctrl, string format)
        {
            EnsurePropertiesExists(ctrl).Format = format;
        }

        private PropertyInfo EnsurePropertiesExists(Control key)
        {
            if (_paths.ContainsKey(key))
            {
                return _paths[key];
            }

            var p = new PropertyInfo();

            _paths.Add(key, p);

            return p;
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            View view = null;
            foreach (var ctrl in _paths.Keys)
            {
                if (view == null) view = GetView(ctrl);

                var pi = _paths[ctrl];
                if (!string.IsNullOrEmpty(pi.ViewModelProperty))
                {
                    try
                    {
                        var binding = view.BindControlProperty(ctrl, pi.ViewModelProperty, pi.Format);
                        if (!string.IsNullOrEmpty(pi.Format))
                        {
                            binding.FormattingEnabled = true;
                            binding.FormatString = pi.Format;
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private View GetView(Control ctrl)
        {
            Control parent = ctrl.Parent;
            while (parent != null && !(parent is View))
            {
                parent = parent.Parent;
            }

            return (View)parent;
        }
    }

    class PropertyInfo
    {
        public string Format { get; set; }
        public string ViewModelProperty { get; set; }
    }
}
