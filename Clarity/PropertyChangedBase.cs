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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using Clarity.Internal;
using System.Linq;

namespace Clarity
{
    [Serializable]
    public abstract class PropertyChangedBase : Disposable, INotifyPropertyChanged
    {
        [NonSerialized]
        private PropertyObserver _propertyObserver;

        [NonSerialized]
        private CollectionObserver _collectionObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedBase"/> class.
        /// </summary>
        public PropertyChangedBase()
        {
            _propertyObserver = new PropertyObserver(this);
            _collectionObserver = new CollectionObserver(this);
        }

        private static bool AreEqual<T>(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        private PropertyObserver PropertyObserver
        {
            get
            {
                if (_propertyObserver == null) _propertyObserver = new PropertyObserver(this);
                return _propertyObserver;
            }
        }

        private CollectionObserver CollectionObserver
        {
            get
            {
                if (_collectionObserver == null) _collectionObserver = new CollectionObserver(this);
                return _collectionObserver;
            }
        }

        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <param name="field">The field to set</param>
        /// <param name="value">The value to set on the field</param>
        /// <param name="property">The property being changed</param>
        /// <param name="executeIfChanged">An <see cref="Action"/> to execute if the property has changed</param>
        protected void SetValue<V, P>(ref V field, V value, Expression<Func<P>> property, Action executeIfChanged = null)
        {
            if (AreEqual(field, value))
            {
                return;
            }

            if (CanChange(property))
            {
                var oldValue = field;
                var propertyName = this.GetPropertyName(property);

                OnPropertyChanging(propertyName, oldValue, value);

                //disconnect any collection change handlers
                if (field != null && field is INotifyCollectionChanged)
                {
                    CollectionObserver.StopObservingCollection(propertyName);
                }

                field = value;

                NotifyPropertyChanged(propertyName, oldValue, value);

                if (executeIfChanged != null)
                {
                    executeIfChanged();
                }

                if (field != null && field is INotifyCollectionChanged)
                {
                    //connect new collection
                    CollectionObserver.ObserveCollection(propertyName, field as INotifyCollectionChanged);
                }
            }
        }

        /// <summary>
        /// Determines whether this instance can change the specified property.
        /// </summary>
        /// <param name="property">The property to test</param>
        /// <returns>true; if the property can be changed</returns>
        protected virtual bool CanChange<T>(Expression<Func<T>> property)
        {
            return true;
        }

        private void NotifyPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            RaiseEvent(propertyName);

            OnPropertyChanged(propertyName, oldValue, newValue);

            PropertyObserver.NotifyObservers(propertyName);
        }

        /// <summary>
        /// Notifies any listeners that the property has changed.
        /// </summary>
        /// <param name="property">The property.</param>
        protected void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            var propertyName = this.GetPropertyName(property);

            RaiseEvent(propertyName);

            PropertyObserver.NotifyObservers(propertyName);
        }

        /// <summary>
        /// Notifies any listeners that the property has changed.
        /// </summary>
        /// <param name="property">The property.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            RaiseEvent(propertyName);

            _propertyObserver.NotifyObservers(propertyName);
        }

        private void RaiseEvent(string propertyName)
        {
            var handler = _propertyChanged;
            if (handler != null)
            {
                var notify = new Action(() => handler(this, new PropertyChangedEventArgs(propertyName)));

                if (Execute.IsOnUIThread)
                {
                    notify();
                }
                else
                {
                    Execute.OnUIThread(() => notify());
                }
            }
        }

        /// <summary>
        /// Called before a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Called when a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Called when a collection has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newItems">The new items.</param>
        protected virtual void OnCollectionChanged(string propertyName, IList originalItems, IList newItems, IList removedItems)
        {
        }

        [NonSerialized]
        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        #region collection management
        /// <summary>
        /// Observes the collection for property changes.
        /// </summary>
        /// <param name="property">The collection property</param>
        protected void ObserveCollection<T>(Expression<Func<T>> property)
        {
            CollectionObserver.ObserveCollection(this.GetPropertyName(property));
        }

        /// <summary>
        /// Locates all collections that are not null and implement the INotifyCollectionChanged interface and monitors each for changes
        /// </summary>
        protected void ObserveCollectionChanges()
        {
            if (ShouldObserveCollectionChanges())
            {
                CollectionObserver.ObserveCollectionChanges();
            }
        }

        /// <summary>
        /// Called from the <see cref="CollectionObserver"/> to indicate it's collection has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="originalItems">The original items.</param>
        internal void NotifyCollectionChanged(string propertyName, List<object> originalItems, List<object> newItems, List<object> removedItems)
        {
            OnCollectionChanged(propertyName, originalItems, newItems, removedItems);
        }

        /// <summary>
        /// Gets a value indicating whether collection changes should be observed.
        /// </summary>
        /// <value>
        /// <c>true</c> if collection changes are being observed; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool ShouldObserveCollectionChanges()
        {
            return true;
        }
        #endregion

        #region observers

        /// <summary>
        /// Allows for the execution of an <see cref="Action"/> when any of the indicated properties have changed
        /// </summary>
        /// <param name="properties">One or more properties that need to be watched</param>
        public Observation<T> OnChangeOf<T>(params Expression<Func<T>>[] properties)
        {
            return new Observation<T>(PropertyObserver, properties.Select(p => p.GetPropertyName(p)));
        }

        /// <summary>
        /// Allows for the execution of an <see cref="Action"/> when any of the indicated properties have changed
        /// </summary>
        /// <param name="propertyNames">One or more properties that need to be watched</param>
        public Observation<object> OnChangeOf(params string[] propertyNames)
        {
            return new Observation<object>(_propertyObserver, propertyNames);
        }

        /// <summary>
        /// Allows for the execution of an <see cref="Action"/> when any property has changed on T
        /// </summary>
        public Observation<T> OnAnyChange<T>()
        {
            return new Observation<T>(PropertyObserver);
        }

        /// <summary>
        /// Removes any observered properties for the given action
        /// </summary>
        /// <param name="action"></param>
        public void RemoveObserversForAction(Action action)
        {
            PropertyObserver.RemoveObserversForAction(action);
        }

        /// <summary>
        /// Clears any observered properties.
        /// </summary>
        public void ClearObservers()
        {
            PropertyObserver.ClearObservers();
        }

        internal void Notify<T>(Expression<Func<T>> p)
        {
            NotifyPropertyChanged(p);
        }

        internal void Notify(string p)
        {
            NotifyPropertyChanged(p);
        }
        #endregion

        protected override void OnDispose()
        {
            base.OnDispose();

            if (PropertyObserver != null)
            {
                PropertyObserver.Dispose();
            }

            if (CollectionObserver != null)
            {
                CollectionObserver.Dispose();
            }

            Logger.Debug("{0} {1} DISPOSED", this.GetType().Name, this.ObjectId);
        }
    }
}
