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
    [ProvideProperty("CommandName", typeof(Button))]
    [ProvideProperty("CommandName", typeof(ToolStripMenuItem))]
    public class ButtonBinding : Component, IExtenderProvider, ISupportInitialize
    {
        private Dictionary<Component, ButtonInfo> _data = new Dictionary<Component, ButtonInfo>();

        public ButtonBinding()
        {
        }

        public ButtonBinding(IContainer cont)
        {
            cont.Add(this);
        }

        public bool CanExtend(object extendee)
        {
            System.Diagnostics.Trace.WriteLine(extendee.GetType().Name);

            return extendee is Button || extendee is Component;
        }

        [Category("Data")]
        [Description("Set to the name of the property on the ViewModel to bind to.")]
        public string GetCommandName(Component btn)
        {
            return EnsurePropertiesExists(btn).CommandName;
        }

        public void SetCommandName(Component btn, string path)
        {
            EnsurePropertiesExists(btn).CommandName = path;
        }

        private ButtonInfo EnsurePropertiesExists(Component key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key];
            }

            var info = new ButtonInfo();

            _data.Add(key, info);

            return info;
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            View view = null;
            foreach (var ctrl in _data.Keys)
            {
                if (view == null) view = GetView(ctrl);
                if (view != null) break;
            }

            if (view != null)
            {
                foreach (var ctrl in _data.Keys)
                {
                    var pi = _data[ctrl];
                    if (!string.IsNullOrEmpty(pi.CommandName))
                    {
                        try
                        {
                            if (ctrl is Button)
                                view.BindButton((Button)ctrl, pi.CommandName);
                            else if (ctrl is ToolStripMenuItem)
                                view.BindMenuItem((ToolStripMenuItem)ctrl, pi.CommandName);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        private View GetView(Component component)
        {
            if (component is Button)
                return GetView((Button)component);

            if (component is ToolStripItem)
            {
                var ts = component as ToolStripItem;
                return GetView(ts.Owner.Parent);
            }

            return null;
        }

        private View GetView(Control ctrl)
        {
            if (ctrl == null) return null;

            if (ctrl.Parent == null)
            {
                if(ctrl is View)
                    return (View)ctrl;

                return null;
            }

            Control parent = ctrl.Parent;
            while (parent != null && !(parent is View))
            {
                parent = parent.Parent;
            }

            return (View)parent;
        }
    }

    class ButtonInfo
    {
        public string CommandName { get; set; }
    }
}
