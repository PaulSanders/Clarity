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

namespace Clarity.Internal
{
    [Serializable]
    internal class CollectionObserver : Disposable
    {
        [NonSerialized]
        private readonly Dictionary<string, INotifyCollectionChanged> _collections = new Dictionary<string, INotifyCollectionChanged>();

        [NonSerialized]
        private PropertyChangedBase _entity;

        public CollectionObserver(PropertyChangedBase entity)
        {
            entity.IfNullThrow("entity");
            _entity = entity;
        }

        public void ObserveCollectionChanges()
        {
            var t = typeof(INotifyCollectionChanged);
            foreach (var property in _entity.GetType().GetProperties())
            {
                if (t.IsAssignableFrom(property.PropertyType))
                {
                    ObserveCollection(property.Name, property.GetValue(_entity, null) as INotifyCollectionChanged);
                }
            }
        }

        public void StopObservingCollection(string propertyName)
        {
            if (_collections.ContainsKey(propertyName))
            {
                var collection = _collections[propertyName];
                collection.CollectionChanged -= OnCollectionChanged;

                _collections.Remove(propertyName);
            }
        }

        public void ObserveCollection(string propertyName)
        {
            var pi = _entity.GetType().GetProperty(propertyName);
            if (pi == null)
            {
                throw new ArgumentException("Property " + propertyName + " not defined");
            }

            if (typeof(INotifyCollectionChanged).IsAssignableFrom(pi.PropertyType))
            {
                var collection = pi.GetValue(_entity, null) as INotifyCollectionChanged;
                if (collection == null)
                {
                    throw new InvalidOperationException("Property " + propertyName + " is null");
                }

                ObserveCollection(propertyName, collection);
            }
        }

        public void ObserveCollection(string propertyName, INotifyCollectionChanged collection)
        {
            if (_entity.GetType().GetProperty(propertyName) == null)
            {
                throw new ArgumentException("Property " + propertyName + " not defined");
            }

            if (!_collections.ContainsKey(propertyName) && collection != null)
            {
                _collections.Add(propertyName, collection);
                collection.CollectionChanged += OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var key in _collections.Keys)
            {
                if (_collections[key] == sender)
                {
                    var collection = sender as IList;
                    var originalItems = new List<object>();
                    var newItems = new List<object>();
                    var oldItems = new List<object>();

                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            newItems.Add(item);
                        }
                    }

                    foreach (var item in collection)
                    {
                        if (e.NewItems != null)
                        {
                            if (!e.NewItems.Contains(item))
                            {
                                originalItems.Add(item);
                            }
                        }
                        else
                        {
                            originalItems.Add(item);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            originalItems.Add(item);
                            oldItems.Add(item);
                        }
                    }

                    _entity.NotifyCollectionChanged(key, originalItems, newItems, oldItems);
                }
            }
        }

        protected override void OnDispose()
        {
            foreach (var key in _collections.Keys)
            {
                _collections[key].CollectionChanged -= OnCollectionChanged;
            }

            _collections.Clear();
            _entity = null;

            base.OnDispose();
        }
    }
}
