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
using System.Linq;
using Clarity.Commands;
using Clarity.Internal;

namespace Clarity
{
    /// <summary>
    /// Allows changes to be tracked
    /// </summary>
    public abstract class TrackingViewModel : ViewModel, IEditableObject, IChangeTracking
    {
        private Dictionary<string, object> _originalValues = new Dictionary<string, object>();
        private HashSet<string> _untracked = new HashSet<string>();

        protected bool ThrowExceptionOnSetWhenNotEditing { get; set; }

        public static bool BeginEditOnConstruction = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingViewModel"/> class.
        /// </summary>
        public TrackingViewModel() : this(BeginEditOnConstruction)
        {
            DontTrack(() => IsClosed);
            DontTrack(() => IsCloseEnabled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingViewModel"/> class.
        /// </summary>
        /// <param name="beginEdit">if set to <c>true</c> editing will be turned on.</param>
        public TrackingViewModel(bool beginEdit)
        {
            ThrowExceptionOnSetWhenNotEditing = true;
            if (beginEdit)
            {
                BeginEdit();
            }

            OnChangeOf(() => IsEditing).Validate(() => IsReadOnly);
        }

        private Initializer _initializer;

        /// <summary>
        /// Allows this instance to be initialized with data.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> that is used for initializing data. Once disosed, initialization is finished. Data can only be initialized once</returns>
        public IDisposable CreateInitializationContext()
        {
            if (_initializer == null)
            {
                _initializer = new Initializer();
            }

            return _initializer;
        }

        /// <summary>
        /// Gets a value indicating whether the model is initializing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is initializing; otherwise, <c>false</c>.
        /// </value>
        protected bool IsInitializing
        {
            get
            {
                return _initializer != null && _initializer.IsDisposed == false;
            }
        }

        #region BeginEdit
        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            if (IsInitializing)
            {
                throw new InvalidOperationException("Cannot begin editing until intialization has finished");
            }

            if (!IsEditing)
            {
                SetEditMode(true);
            }
        }

        private IClarityCommand _beginEdit;
        public IClarityCommand BeginEditCommand
        {
            get
            {
                if (_beginEdit == null)
                {
                    _beginEdit = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteBeginEdit, CanExecuteBeginEdit);
                }
                return _beginEdit;
            }
        }

        private bool CanExecuteBeginEdit()
        {
            return !IsEditing;
        }

        private void ExecuteBeginEdit()
        {
            BeginEdit();
        }
        #endregion

        /// <summary>
        /// Clears all changes to the model.
        /// </summary>
        public void ClearChanges()
        {
            _originalValues.Clear();
            NotifyPropertyChanged(() => IsChanged);
        }

        /// <summary>
        /// Indicates that the given property is not recorded in change history
        /// </summary>
        /// <typeparam name="T">The current type</typeparam>
        /// <param name="property">The property.</param>
        protected void DontTrack<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            _untracked.Add(this.GetPropertyName(property));
        }

        /// <summary>
        /// Determines whether the specified property name is untracked.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>true; if untracked</returns>
        protected bool IsUntracked(string propertyName)
        {
            //this can be null when the object is being reconstructed from serialized data
            if (_untracked == null) _untracked = new HashSet<string>();

            return _untracked.Contains(propertyName);
        }

        /// <summary>
        /// Called before the property is changed
        /// </summary>
        /// <param name="propertyName">Name of the property changing</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        protected override void OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
            if (!IsInitializing)
            {
                if (!IsUntracked(propertyName))
                {
                    Verify(propertyName);
                }
            }

            base.OnPropertyChanging(propertyName, oldValue, newValue);
        }

        /// <summary>
        /// Determines whether this instance can change the specified property.
        /// </summary>
        /// <param name="property">The property to test</param>
        /// <returns>true; if the property can be changed</returns>
        protected override bool CanChange<T>(System.Linq.Expressions.Expression<Func<T>> property)
        {
            if (IsInitializing)
            {
                return true;
            }

            if (_inCancel || IsUntracked(this.GetPropertyName(property)))
            {
                return true;
            }

            Verify(this.GetPropertyName(property));

            return IsEditing;
        }

        /// <summary>
        /// Called when a property has changed
        /// </summary>
        /// <param name="propertyName">Name of the property changing</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            bool changed = false;

            if (!IsInitializing && !IsUntracked(propertyName))
            {
                Verify(propertyName);
            }

            if (IsEditing)
            {
                if (false == _originalValues.ContainsKey(propertyName))
                {
                    if (!IsUntracked(propertyName) && !IsInitializing)
                    {
                        _originalValues.Add(propertyName, oldValue);
                    }

                    changed = true;
                }

                base.OnPropertyChanged(propertyName, oldValue, newValue);

                if (changed || IsInitializing)
                {
                    NotifyPropertyChanged(() => IsChanged);
                }
            }
        }

        /// <summary>
        /// Called when a collection has changed.
        /// </summary>
        /// <param name="propertyName">Name of the collection property.</param>
        /// <param name="originalItems">The original items.</param>
        protected override void OnCollectionChanged(string propertyName, System.Collections.IList originalItems, System.Collections.IList newItems, System.Collections.IList removedItems)
        {
            bool changed = false;
            if (!IsInitializing)
            {
                Verify(propertyName);
            }

            if (false == _originalValues.ContainsKey(propertyName))
            {
                if (!IsUntracked(propertyName) && !IsInitializing)
                {
                    _originalValues.Add(propertyName, originalItems);
                }

                changed = true;
            }

            base.OnCollectionChanged(propertyName, originalItems, newItems, removedItems);

            if (changed)
            {
                NotifyPropertyChanged(() => IsChanged);
            }
        }

        #region CancelEdit
        private bool _inCancel;

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
        /// </summary>
        public void CancelEdit()
        {
            if (IsEditing && !_inCancel)
            {
                _inCancel = true;
                foreach (var key in _originalValues.Keys)
                {
                    var prop = this.GetType().GetProperty(key);

                    //collections are a special case
                    if (typeof(System.Collections.IList).IsAssignableFrom(prop.PropertyType))
                    {
                        var collection = prop.GetValue(this, null) as System.Collections.IList;
                        collection.Clear();

                        foreach (var item in (System.Collections.IList)_originalValues[key])
                        {
                            collection.Add(item);
                        }
                    }
                    else if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(this, _originalValues[key], null);
                    }
                }

                _originalValues.Clear();

                SetEditMode(false);
                _inCancel = false;

                NotifyPropertyChanged(() => IsEditing);
                NotifyPropertyChanged(() => IsChanged);
            }
        }

        private IClarityCommand _cancelEdit;
        public IClarityCommand CancelEditCommand
        {
            get
            {
                if (_cancelEdit == null)
                {
                    _cancelEdit = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteCancelEdit, CanExecuteCancelEdit);
                }
                return _cancelEdit;
            }
        }

        private bool CanExecuteCancelEdit()
        {
            return IsEditing && !_inCancel;
        }

        private void ExecuteCancelEdit()
        {
            CancelEdit();
        }
        #endregion

        #region EndEdit
        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> or <see cref="M:System.ComponentModel.IBindingList.AddNew" /> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            AcceptChanges();
        }

        private IClarityCommand _endEdit;
        public IClarityCommand EndEditCommand
        {
            get
            {
                if (_endEdit == null)
                {
                    _endEdit = ServiceManager.Default.Resolve<ICommandBuilder>().BuildSimple(ExecuteEndEdit, CanExecuteEndEdit);
                }
                return _endEdit;
            }
        }

        private bool CanExecuteEndEdit()
        {
            return IsEditing && !_inCancel;
        }

        private void ExecuteEndEdit()
        {
            AcceptChanges();
        }

        /// <summary>
        /// Called when editing is trying to end.
        /// </summary>
        /// <returns>true; if ok to end the edit process</returns>
        protected virtual bool OnEndEdit()
        {
            return true;
        }
        #endregion

        private void SetEditMode(bool value)
        {
            _isEditing = value;
            NotifyPropertyChanged(() => IsEditing);
        }

        private bool _isEditing;

        /// <summary>
        /// Gets a value indicating whether the model is being edited.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is editing]; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing
        {
            get
            {
                return _isEditing;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return !_isEditing;
            }
        }

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <returns>true if the object’s content has changed since the last call to <see cref="M:System.ComponentModel.IChangeTracking.AcceptChanges" />; otherwise, false.</returns>
        public bool IsChanged
        {
            get
            {
                return _originalValues.Count > 0;
            }
        }

        /// <summary>
        /// Gets the original values before any changes had ocurred.
        /// </summary>
        public Dictionary<string, object> OriginalValues
        {
            get
            {
                return _originalValues;
            }
        }

        /// <summary>
        /// Gets a list of changed properties.
        /// </summary>
        public IList<string> GetChangedProperties()
        {
            return new List<string>(_originalValues.Keys);
        }

        /// <summary>
        /// Gets a list of changes.
        /// </summary>
        public Dictionary<string, object> GetChanges()
        {
            var dict = new Dictionary<string, object>();
            var properties = GetType().GetProperties().ToDictionary((p) => p.Name);

            foreach (var key in _originalValues.Keys)
            {
                var prop = properties[key];
                dict.Add(key, prop.GetValue(this, null));
            }

            return dict;
        }

        private void Verify(string propertyName)
        {
            if (IsEditing)
            {
                return;
            }

            if (ThrowExceptionOnSetWhenNotEditing)
            {
                throw new InvalidOperationException("Cannot set property " + propertyName + " when not editing");
            }
        }

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public void AcceptChanges()
        {
            if (OnEndEdit())
            {
                _originalValues.Clear();
                SetEditMode(false);
            }
        }
    }
}
