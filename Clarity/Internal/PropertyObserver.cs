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
using System.Linq;

namespace Clarity.Internal
{

    /// <summary>
    /// Observes property changes and notifies identified listeners
    /// </summary>
    [Serializable]
    internal class PropertyObserver : Disposable
    {
        [NonSerialized]
        private PropertyChangedBase _owner;

        [NonSerialized]
        private readonly Dictionary<string, HashSet<ActionInfo>> _observers = new Dictionary<string, HashSet<ActionInfo>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyObserver"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public PropertyObserver(PropertyChangedBase owner)
        {
            owner.IfNullThrow("owner");
            _owner = owner;
        }

        /// <summary>
        /// Observes the specified action.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="properties">The properties to monitor</param>
        /// <exception cref="System.ArgumentNullException">properties;At least 1 property must be specified</exception>
        public void Observe(Action action, IEnumerable<string> properties, TimeSpan delayTime, bool onUIThread)
        {
            action.IfNullThrow("action");
            if (properties == null || !properties.Any())
            {
                throw new ArgumentNullException("properties", "At least 1 property must be specified");
            }

            Logger.Debug("Observing action " + action.Method.Name + " on " + _owner.GetType().Name + " [" + _owner.ObjectId + "]");
            foreach (var propertyName in properties)
            {
                if (!_observers.ContainsKey(propertyName))
                {
                    _observers.Add(propertyName, new HashSet<ActionInfo>());
                }

                if (delayTime == TimeSpan.Zero)
                    _observers[propertyName].Add(new ActionInfo(action) { ExecuteOnUIThread = onUIThread });
                else
                    _observers[propertyName].Add(new ActionInfo(action, delayTime) { ExecuteOnUIThread = onUIThread });
            }
        }

        /// <summary>
        /// Observes all.
        /// </summary>
        /// <param name="type">Listens to changes from all properties of the defined type only.</param>
        /// <param name="action">The action to execute</param>
        public void ObserveAll(Type type, Action action, TimeSpan delayTime, bool onUIThread)
        {
            action.IfNullThrow("action");

            Logger.Debug("Observing action " + action.Method.Name + " on " + _owner.GetType().Name + " [" + _owner.ObjectId + "]");
            foreach (var property in type.GetProperties().Where(p => p.CanRead && p.CanWrite && p.DeclaringType == type))
            {
                var propertyName = property.Name;

                if (!_observers.ContainsKey(propertyName))
                {
                    _observers.Add(propertyName, new HashSet<ActionInfo>());
                }

                if (delayTime == TimeSpan.Zero)
                    _observers[propertyName].Add(new ActionInfo(action) { ExecuteOnUIThread = onUIThread });
                else
                    _observers[propertyName].Add(new ActionInfo(action, delayTime) { ExecuteOnUIThread = onUIThread });
            }
        }

        internal void NotifyOnChange(IEnumerable<string> propertiesToMonitor, IEnumerable<string> propertiesToNotify)
        {
            propertiesToNotify.IfNullThrow("propertiesToNotify");
            propertiesToMonitor.IfNullThrow("propertiesToMonitor");

            foreach (var propertyName in propertiesToMonitor)
            {
                if (!_observers.ContainsKey(propertyName))
                {
                    _observers.Add(propertyName, new HashSet<ActionInfo>());
                }

                _observers[propertyName].Add(new ActionInfo(() => Notify(propertiesToNotify)));
            }
        }

        internal void NotifyOnAnyChange(Type t, IEnumerable<string> propertiesToNotify)
        {
            propertiesToNotify.IfNullThrow("propertiesToNotify");

            foreach (var property in t.GetProperties().Where(p => p.CanRead && p.CanWrite && p.DeclaringType == t))
            {
                var propertyName = property.Name;
                if (!_observers.ContainsKey(propertyName))
                {
                    _observers.Add(propertyName, new HashSet<ActionInfo>());
                }

                _observers[propertyName].Add(new ActionInfo(() => Notify(propertiesToNotify)));
            }
        }

        private bool _inPropertyNotify = false;
        private void Notify(IEnumerable<string> properties)
        {
            properties.IfNullThrow("properties");

            if (!_inPropertyNotify)
            {
                _inPropertyNotify = true;
                foreach (var p in properties)
                {
                    _owner.Notify(p);
                }

                _inPropertyNotify = false;
            }
        }

        /// <summary>
        /// Removes any observered properties for the given action
        /// </summary>
        /// <param name="action"></param>
        public void RemoveObserversForAction(Action action)
        {
            action.IfNullThrow("action");

            foreach (var key in _observers.Keys)
            {
                var observers = _observers[key];
                var list = new List<ActionInfo>();

                foreach (var ai in observers)
                {
                    if (ai.Action == action) list.Add(ai);
                }

                foreach (var ai in list)
                {
                    observers.Remove(ai);
                }
            }
        }

        /// <summary>
        /// Clears all observers from the dictionary
        /// </summary>
        public void ClearObservers()
        {
            _observers.Clear();
        }

        /// <summary>
        /// Execute all actions that are monitoring the given property
        /// </summary>
        /// <param name="propertyName">Name of the property to check</param>
        public void NotifyObservers(string propertyName)
        {
            ClearDisposedObservers();

            if (_observers.ContainsKey(propertyName))
            {
                Logger.Debug("Notify observers of " + propertyName);

                var observers = _observers[propertyName];

                foreach (var observer in observers)
                {
                    if (observer.ExecuteOnUIThread)
                        Execute.OnUIThread(observer.Execute);
                    else
                        observer.Execute();
                }
            }
        }

        private void ClearDisposedObservers()
        {
            foreach (var key in _observers.Keys)
            {
                var observers = _observers[key];
                observers.RemoveWhere((a) =>
                    {
                        bool isDisposed = a.Action.Target is Disposable ? ((Disposable)a.Action.Target).IsDisposed : false;
                        if (isDisposed) a.Dispose();

                        return isDisposed;
                    });
            }
        }

        protected override void OnDispose()
        {
            foreach (var key in _observers.Keys)
            {
                var observers = _observers[key];
                foreach (var observer in observers) observer.Dispose();
            }

            _observers.Clear();
            _owner = null;

            base.OnDispose();
        }
    }
}
