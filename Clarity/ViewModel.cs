using Clarity.Commands;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Clarity
{
    public abstract class ViewModel : PropertyChangedBase, IClosable, IDataErrorInfo, IValidatableObject
    {
        public ViewModel()
        {
        }

        private string _title;
        public virtual string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged(() => Title);
                }
            }
        }

        #region command helpers
        protected IClarityCommand BuildSimpleCommand(Action execute)
        {
            return ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(execute);
        }

        protected IClarityCommand BuildSimpleCommand(Action execute, Func<bool> canExecute)
        {
            return ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(execute, canExecute);
        }

		protected IClarityCommand BuildSimpleAsyncCommand(Action execute)
		{
			return ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimpleAsync(execute);
		}

		protected IClarityCommand BuildSimpleAsyncCommand(Action execute, Func<bool> canExecute)
		{
			return ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimpleAsync(execute, canExecute);
		}

		protected IClarityCommand BuildDelegateCommand<T>(Action<T> execute)
		{
			return ServiceManager.Default.Resolve<ICommandBuilder>().BuildDelegate<T>(execute);
		}

		protected IClarityCommand BuildDelegateCommand<T>(Action<T> execute, Func<T, bool> canExecute)
		{
			return ServiceManager.Default.Resolve<ICommandBuilder>().BuildDelegate<T>(execute, canExecute);
		}

		protected IClarityCommand BuildDelegateAsyncCommand<T>(Action<T> execute)
        {
            return ServiceManager.Default.Resolve<ICommandBuilder>().BuildDelegateAsync<T>(execute);
        }

        protected IClarityCommand BuildDelegateAsyncCommand<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            return ServiceManager.Default.Resolve<ICommandBuilder>().BuildDelegateAsync<T>(execute, canExecute);
        }
        #endregion

        #region Close Management
        private bool _isCloseEnabled;
        public virtual bool IsCloseEnabled
        {
            get
            {
                return _isCloseEnabled;
            }

            set
            {
                SetValue(ref _isCloseEnabled, value, () => IsCloseEnabled);
            }
        }

        private IClarityCommand _close;
        public IClarityCommand CloseCommand
        {
            get
            {
                if (_close == null)
                {
                    _close = BuildSimpleCommand(ExecuteClose);
                }

                return _close;
            }
        }

        private void ExecuteClose()
        {
            if (CanClose())
            {
                OnClose();
            }
        }

        /// <summary>
        /// Called when CloseCommand has been executed.
        /// </summary>
        protected virtual void OnClose()
        {
            Close();
        }

        private bool _isClosed;

        /// <summary>
        /// Gets a value indicating whether the ViewMode has closed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is closed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }

            private set
            {
                SetValue(ref _isClosed, value, () => IsClosed);
            }
        }

        private bool _displayResult;

        /// <summary>
        /// Gets or sets a value indicating whether display of the viewmodel was successful
        /// </summary>
        public virtual bool DisplayResult
        {
            get
            {
                return _displayResult;
            }

            set
            {
                SetValue(ref _displayResult, value, () => DisplayResult);

                //by calling Close, this emulates the same functionality of setting the DialogResult of a Window
                Close();
            }
        }

        /// <summary>
        /// Determines whether this instance can close.
        /// </summary>
        public virtual bool CanClose()
        {
            return true;
        }

        /// <summary>
        /// Closes this instance. OnClose and CanClose will not be executed
        /// </summary>
        public virtual void Close()
        {
            if (!IsClosed || !IsDisposed)
            {
                IsClosed = true;
                Dispose();
            }
        }
        #endregion

        #region instance disposal
        protected override void OnDispose()
        {
            base.OnDispose();
            DisposeCommands();
            DisposeChildViewModels();
        }

        private void DisposeCommands()
        {
            foreach (var prop in this.GetType().GetProperties().Where(p => typeof(IClarityCommand).IsAssignableFrom(p.PropertyType)))
            {
                var cmd = prop.GetValue(this, null) as IDisposable;
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }

        private void DisposeChildViewModels()
        {
            foreach (var prop in this.GetType().GetProperties().Where(p => typeof(ViewModel).IsAssignableFrom(p.PropertyType)))
            {
                var vm = prop.GetValue(this, null) as IDisposable;
                if (vm != null)
                {
                    vm.Dispose();
                }
            }

            foreach (var prop in this.GetType().GetProperties().Where(p => typeof(IEnumerable<ViewModel>).IsAssignableFrom(p.PropertyType)))
            {
                var items = prop.GetValue(this, null) as IEnumerable<ViewModel>;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (item != null) item.Dispose();
                    }
                }
            }
        }
        #endregion

        #region validation
        private ValidationResults _results;

        /// <summary>
        /// Performs validation and returns the result
        /// </summary>
        /// <returns>true if valid; otherwise false</returns>
        public bool IsValid()
        {
            return _results == null || _results.Count == 0;
        }

        /// <summary>
        /// Gets the collection of errors currently known.
        /// </summary>
        public IEnumerable<string> ErrorCollection
        {
            get
            {
                return Validate(string.Empty).Select(em => em.ErrorMessage);
            }
        }

        /// <summary>
        /// Returns all known errors as a composite string
        /// </summary>
        public string Error
        {
            get
            {
                var error = new StringBuilder();

                foreach (var result in Validate(string.Empty))
                {
                    error.AppendLine(result.ErrorMessage);
                }

                return error.ToString();
            }
        }

        /// <summary>
        /// Returns an error message for a given property
        /// </summary>
        /// <param name="propertyName">Name of the property to check</param>
        /// <returns>A populated string if there is an error, otherwise String.Empty</returns>
        public string this[string propertyName]
        {
            get
            {
                return Validate(propertyName).GetValidationError(propertyName);
            }
        }

        private ValidationResults Validate(string propertyName)
        {
            var results = OnValidate(propertyName);
            HasErrors = results != null && results.Count > 0;

            return results;
        }

        /// <summary>
        /// Perform validation and return a <see cref="ValidationResults"/> instance
        /// </summary>
        public virtual ValidationResults OnValidate(string propertyName)
        {
            if (_results == null)
            {
                _results = ValidationResults.CreateFromAttributes(this);

                foreach (var additionalResult in AdditionalErrors.Values)
                {
                    _results.Add(additionalResult);
                }
            }

            return _results;
        }

        private Dictionary<string, ValidationResult> _additionalErrors = new Dictionary<string, ValidationResult>();
        private Dictionary<string, ValidationResult> AdditionalErrors
        {
            get
            {
                if (_additionalErrors == null) _additionalErrors = new Dictionary<string, ValidationResult>();
                return _additionalErrors;
            }
        }

        public void AddSourceValidationError(ValidationResult validationResult)
        {
            validationResult.IfNullThrow("validationResult");

            var key = validationResult.MemberNames.First();

            if (AdditionalErrors.ContainsKey(key))
            {
                AdditionalErrors.Remove(key);
            }

            AdditionalErrors.Add(key, validationResult);
            IsValid();
        }

        private bool _hasErrors;

        /// <summary>
        /// Returns true if the data contained withing the ViewModel is not valid
        /// </summary>
        public virtual bool HasErrors
        {
            get
            {
                return _hasErrors;
            }

            set
            {
                if (_hasErrors != value)
                {
                    _hasErrors = value;
                    NotifyPropertyChanged(() => HasErrors);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }

            return OnValidate(validationContext.MemberName);
        }

        /// <summary>
        /// Called before property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            base.OnPropertyChanging(propertyName, oldValue, newValue);
            _results = null;
        }

        /// <summary>
        /// Called when a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            _results = null;

            base.OnPropertyChanged(propertyName, oldValue, newValue);

            if (AdditionalErrors.ContainsKey(propertyName))
            {
                AdditionalErrors.Remove(propertyName);
            }
        }

        #endregion
    }
}
