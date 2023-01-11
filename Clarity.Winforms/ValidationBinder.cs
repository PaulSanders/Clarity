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
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace Clarity.Winforms
{
    public class ValidationBinder : Disposable
    {
        private ViewModel _viewModel;
        private View _view;
        private ViewModel[] _models;

        /// <summary>
        /// Iterates through each model, binds to any changes in the HasErrors property and maps those errors
        /// to the ErrorProvider on the view
        /// </summary>
        /// <param name="view">The view to bind</param>
        /// <param name="models">One or more models exposed on the ViewModel</param>
        public void BindModelErrors(View view, params ViewModel[] models)
        {
            view.IfNullThrow("view");
            view.ViewModel.IfNullThrow("view.ViewModel");
            models.IfNullThrow("models");

            if (models.Length > 0)
            {
                _view = view;
                _viewModel = view.ViewModel;

                _models = models;

                foreach (var model in models)
                {
                    MonitorValidationErrors(model);
                }

                _viewModel.OnChangeOf(() => _viewModel.IsClosed).Execute(() => Dispose());
            }
        }

        private void MonitorValidationErrors(ViewModel model)
        {
            model.OnChangeOf(() => model.HasErrors).Execute(() =>
            {
                var ep = _view.GetErrorProvider();
                if (model.HasErrors)
                {
                    foreach (var e in model.Validate(new ValidationContext(model, null, null)))
                    {
                        foreach (var control in GetControlsForMember(e.MemberNames))
                        {
                            ep.SetError(control, e.ErrorMessage);
                        }
                    }
                }
                else
                {
                    ep.Clear();
                }
            });
        }

        private IList<Control> GetControlsForMember(IEnumerable<string> memberNames)
        {
            var controls = new List<Control>();

            foreach (Control control in _view.Controls)
            {
                foreach (var mn in memberNames)
                {
                    if (control is ComboBox)
                    {
                        var binding = _view.GetItemsControlBinding(control);
                        if (binding != null)
                        {
                            if (mn == binding.LastProperty) controls.Add(control);
                        }
                    }
                    else
                    {
                        foreach (Binding binding in control.DataBindings)
                        {
                            if (binding.BindingMemberInfo.BindingField == mn) controls.Add(control);
                        }
                    }
                }
            }

            return controls;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _view = null;
            _viewModel = null;
            _models = null;
        }
    }
}
