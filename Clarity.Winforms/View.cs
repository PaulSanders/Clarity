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
using Clarity.Commands;
using Clarity.Winforms.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Clarity.Winforms
{
    public class View : UserControl
    {
        private ErrorProvider _errorProvider;
        private readonly Dictionary<Control, ItemsControlBindingData> _itemsControlBindings = new Dictionary<Control, ItemsControlBindingData>();

        public View()
        {
        }

        internal ErrorProvider GetErrorProvider()
        {
            if (_errorProvider == null)
            {
                _errorProvider = new ErrorProvider(this);
            }
            return _errorProvider;
        }

        #region binding
        private List<DelayedBindingData> _delayedBindingData;

        public virtual void BindViewModel()
        {
            EnsureViewModel();

            if (ParentWindow == null) return;

            if (_delayedBindingData != null)
            {
                foreach (var item in _delayedBindingData)
                {
                    if (string.IsNullOrEmpty(item.CommandName))
                    {
                        BindControlProperty((Control)item.Control, item.ViewModelProperty, item.Format);
                    }
                    else
                    {
                        if (item.Control is Button)
                            BindButton((Button)item.Control, item.CommandName);
                        else if(item.Control is ToolStripMenuItem)
                            BindMenuItem((ToolStripMenuItem)item.Control, item.CommandName);
                    }
                }

                _delayedBindingData.Clear();
            }
        }

        public Binding BindControlProperty(Control ctrl, string propertyPath, string format = null)
        {
            return BindControlProperty(ctrl, GetDefaultControlBindingProperty(ctrl.GetType()), propertyPath, format);
        }

        public void BindItemsControl(ComboBox cbo, string displayMember, string dataSource, string selectedPath, IValueConverter converter = null)
        {
            //Combobox selected item keep resetting to the original value...
            //BindControlProperty(cbo, "DataSource", dataSource, null);
            //cbo.SelectionChangeCommitted += (o, e) =>
            //    {
            //         ViewModel.SetProperty(selectedPath, cbo.SelectedItem);
            //    };

            //cbo.DisplayMember = displayMember;
            if (_itemsControlBindings.ContainsKey(cbo)) return;

            cbo.Items.Clear();
            if (!string.IsNullOrEmpty(displayMember)) cbo.DisplayMember = displayMember;
            foreach (var item in (IEnumerable)ViewModel.GetProperty(dataSource))
            {
                cbo.Items.Add(item);
            }

            PropertyChangedBase actualModel = ViewModel;

            if (selectedPath.Contains("."))
            {
                var dataSources = selectedPath.Split('.');

                for (int i = 0; i < dataSources.Length - 1; i++)
                {
                    actualModel = actualModel.GetProperty(dataSources[i]) as PropertyChangedBase;
                    if (actualModel == null)
                    {
                        throw new ArgumentException("SelectedPath must inherit PropertyChangedBase.");
                    }
                }

                selectedPath = dataSources.Last();

                actualModel.OnChangeOf(dataSources.Last()).Execute(() =>
                {
                    var value = actualModel.GetProperty(selectedPath);
                    if (value != cbo.SelectedItem)
                    {
                        cbo.SelectedItem = value;
                    }
                });
            }
            else
            {
                actualModel.OnChangeOf(selectedPath).Execute(() =>
                    {
                        var value = actualModel.GetProperty(selectedPath);
                        if (value != cbo.SelectedItem)
                        {
                            cbo.SelectedItem = value;
                        }
                    });
            }

            var vmValue = actualModel.GetProperty(selectedPath);

            if (converter == null)
            {
                cbo.SelectedIndex = cbo.Items.IndexOf(vmValue);
            }
            else
            {
                cbo.SelectedIndex = cbo.Items.IndexOf(converter.Convert(vmValue, typeof(string), null, System.Threading.Thread.CurrentThread.CurrentCulture));
            }

            cbo.SelectedIndexChanged += (o, e) =>
            {
                object newValue = null;
                if (converter != null)
                {
                    if (cbo.SelectedIndex == -1)
                        newValue = null;
                    else
                        newValue = converter.ConvertBack(cbo.Items[cbo.SelectedIndex], actualModel.GetProperty(selectedPath).GetType(), null, System.Threading.Thread.CurrentThread.CurrentCulture);
                }
                else
                {
                    if (cbo.SelectedIndex != -1)
                    {
                        newValue = cbo.Items[cbo.SelectedIndex];
                    }
                }
                actualModel.SetProperty(selectedPath, newValue);
            };

            _itemsControlBindings.Add(cbo, new ItemsControlBindingData(cbo, selectedPath, converter));
        }

        public Binding BindControlProperty(Control ctrl, string controlProperty, string propertyPath, string format)
        {
            ctrl.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            if (ParentWindow == null)
            {
                if (_delayedBindingData == null) _delayedBindingData = new List<DelayedBindingData>();
                _delayedBindingData.Add(new DelayedBindingData() { Control = ctrl, ControlProperty = controlProperty, ViewModelProperty = propertyPath, Format = format });

                return null;
            }
            else
            {
                var binding = ctrl.DataBindings.Add(controlProperty, ViewModel, propertyPath);
                if (!string.IsNullOrEmpty(format))
                {
                    binding.FormattingEnabled = true;
                    binding.FormatString = format;
                }
                return binding;
            }
        }

        protected string GetDefaultControlBindingProperty(Type controlType)
        {
            if (typeof(Label) == controlType) return "Text";
            if (typeof(TextBox) == controlType) return "Text";
            if (typeof(NumericUpDown) == controlType) return "Value";
            if (typeof(DateTimePicker) == controlType) return "Value";
            if (typeof(CheckBox) == controlType) return "Checked";
            if (typeof(RadioButton) == controlType) return "Checked";
            if (typeof(ProgressBar) == controlType) return "Value";
            if (typeof(Label) == controlType) return "Checked";

            return null;
        }

        internal void BindButton(Button btn, string commandName)
        {
            if (ViewModel == null)
            {
                if (_delayedBindingData == null) _delayedBindingData = new List<DelayedBindingData>();
                _delayedBindingData.Add(new DelayedBindingData() { Control = btn, CommandName = commandName });
            }
            else
            {
                var vmType = ViewModel.GetType();
                var pi = vmType.GetProperty(commandName);
                if (pi == null) throw new ArgumentException("Command '" + commandName + "' could not be found on ViewModel '" + vmType.Name + "'");
                if (typeof(IClarityCommand).IsAssignableFrom(pi.PropertyType))
                {
                    IClarityCommand cmd = (IClarityCommand)pi.GetValue(ViewModel, null);
                    BindButton(btn, cmd, null);
                }
                else
                {
                    throw new ArgumentException("The ViewModel property '" + commandName + "' must implement IClarityCommand");
                }
            }
        }

        public void BindButton(Button btn, IClarityCommand cmd, object value = null)
        {
            cmd.CanExecuteChanged += (o, e) => btn.Enabled = cmd.CanExecute(value);
            btn.Click += (o, e) => cmd.Execute(value);
        }

        public void BindButton(RadioButton btn, IClarityCommand cmd, object value = null)
        {
            cmd.CanExecuteChanged += (o, e) => btn.Enabled = cmd.CanExecute(value);
            btn.Click += (o, e) => cmd.Execute(value);
        }

        public void BindButton(ToolStripButton btn, IClarityCommand cmd, object value = null)
        {
            cmd.CanExecuteChanged += (o, e) => btn.Enabled = cmd.CanExecute(value);
            btn.Click += (o, e) => cmd.Execute(value);
        }

        public void BindMenuItem(ToolStripMenuItem menu,IClarityCommand cmd, object value = null)
        {
            cmd.CanExecuteChanged += (o, e) => menu.Enabled = cmd.CanExecute(value);
            menu.Click += (o, e) => cmd.Execute(value);
        }

        internal void BindMenuItem(ToolStripMenuItem btn, string commandName)
        {
            if (ViewModel == null)
            {
                if (_delayedBindingData == null) _delayedBindingData = new List<DelayedBindingData>();
                _delayedBindingData.Add(new DelayedBindingData() { Control = btn, CommandName = commandName });
            }
            else
            {
                var vmType = ViewModel.GetType();
                var pi = vmType.GetProperty(commandName);
                if (pi == null) throw new ArgumentException("Command '" + commandName + "' could not be found on ViewModel '" + vmType.Name + "'");
                if (typeof(IClarityCommand).IsAssignableFrom(pi.PropertyType))
                {
                    IClarityCommand cmd = (IClarityCommand)pi.GetValue(ViewModel, null);
                    BindMenuItem(btn, cmd, null);
                }
                else
                {
                    throw new ArgumentException("The ViewModel property '" + commandName + "' must implement IClarityCommand");
                }
            }
        }
        #endregion

        private void EnsureViewModel()
        {
            if (AutoCreateViewModel && ViewModel == null && ViewModelType != null)
            {
                ViewModel = (ViewModel)ServiceManager.Default.Resolve(ViewModelType);
            }
        }

        private ViewModel _viewModel;
        public virtual ViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    BindViewModel();
                }
            }
        }

        public MvvmWindow ParentWindow
        {
            get
            {
                return (MvvmWindow)ParentForm;
            }
        }

        internal ItemsControlBindingData GetItemsControlBinding(Control ctrl)
        {
            if (_itemsControlBindings.ContainsKey(ctrl))
                return _itemsControlBindings[ctrl];

            return null;
        }

        private bool _autoCreateViewModel;
        [Category("Data")]
        public virtual bool AutoCreateViewModel
        {
            get
            {
                return _autoCreateViewModel;
            }
            set
            {
                if (value != _autoCreateViewModel)
                {
                    _autoCreateViewModel = value;
                }
            }
        }

        private Type _viewModelType;
        [Category("Data")]
        public virtual Type ViewModelType
        {
            get
            {
                return _viewModelType;
            }
            set
            {
                if (value == null || typeof(ViewModel).IsAssignableFrom(value))
                {
                    _viewModelType = value;
                }
            }
        }

        protected V CreateView<V,VM>() where V : View where VM : ViewModel
        {
            var view = ServiceManager.Default.Resolve<IViewLocator>().LocateView<V>(typeof(VM), typeof(V).Assembly);
            view.ViewModel = ServiceManager.Default.Resolve<VM>();

            return view;
        }
    }

    class DelayedBindingData
    {
        public Component Control { get; set; }
        public string ControlProperty { get; set; }
        public string ViewModelProperty { get; set; }
        public string Format { get; set; }
        public string CommandName { get; set; }

    }

    internal class ItemsControlBindingData
    {
        public ItemsControlBindingData(Control ctrl, string propertyPath, IValueConverter converter)
        {
            ctrl.IfNullThrow("ctrl");
            propertyPath.IfNullThrow("propertyPath");

            Control = ctrl;
            PropertyPath = propertyPath;
            Converter = converter;
        }

        public Control Control { get; private set; }
        public string PropertyPath { get; private set; }
        public IValueConverter Converter { get; private set; }

        public string LastProperty
        {
            get
            {
                if (PropertyPath.Contains("."))
                {
                    return PropertyPath.Substring(PropertyPath.LastIndexOf(".") + 1);
                }

                return PropertyPath;
            }
        }
    }
}
