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
using System.Linq.Expressions;
using Clarity.Internal;

namespace Clarity
{
    /// <summary>
    /// Observation class. Used by <see cref="PropertyChangedBase"/> for fluent syntax only
    /// </summary>
    /// <typeparam name="T">The type being observed</typeparam>
    public sealed class Observation<T>
    {
        private IEnumerable<string> _propertyNames;
        private PropertyObserver _parent;

        private Observation()
        {
        }

        internal Observation(PropertyObserver parent, IEnumerable<string> properties)
        {
            parent.IfNullThrow("parent");
            if (properties == null || !properties.Any())
            {
                throw new ArgumentNullException("properties");
            }

            _propertyNames = properties;
            _parent = parent;
        }

        internal Observation(PropertyObserver parent)
        {
            parent.IfNullThrow("parent");

            _parent = parent;
        }

        /// <summary>
        /// Specifies an action to execute for this observation
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <returns>This instance</returns>
        public Observation<T> Execute(Action action)
        {
            return ExecuteAfterDelay(action, TimeSpan.Zero);
        }

        /// <summary>
        /// Specifies an action to execute for this observation. The action will be executed after the specified delay
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="afterDelay">The amount of time to wait before executing the notification</param>
        /// <param name="onUIThread">Indicates if the action should be executed on the STA thread</param>
        /// <returns>This instance</returns>
        public Observation<T> ExecuteAfterDelay(Action action, TimeSpan afterDelay, bool onUIThread = true)
        {
            action.IfNullThrow("action");
            if (_propertyNames == null)
                _parent.ObserveAll(typeof(T), action, afterDelay, onUIThread);
            //else if(_properties != null)
            //    _parent.Observe(action, _properties, afterDelay, onUIThread);
            else if (_propertyNames != null)
                _parent.Observe(action, _propertyNames, afterDelay, onUIThread);

            return this;
        }

        /// <summary>
        /// Re-validates the specified properties when this observation is triggered
        /// </summary>
        /// <param name="properties">The properties to validate</param>
        /// <returns>This instance</returns>
        public Observation<T> Validate<P>(params Expression<Func<P>>[] properties)
        {
            return Validate<P>(properties.Select(p => this.GetPropertyName(p)).ToArray());
        }

        /// <summary>
        /// Re-validates the specified properties when this observation is triggered
        /// </summary>
        /// <param name="properties">The properties to validate</param>
        /// <returns>This instance</returns>
        public Observation<T> Validate<P>(params string[] propertyNames)
        {
            if (_propertyNames == null)
                _parent.NotifyOnAnyChange(typeof(T), propertyNames);
            else
                _parent.NotifyOnChange(_propertyNames, propertyNames);

            return this;
        }
    }
}
