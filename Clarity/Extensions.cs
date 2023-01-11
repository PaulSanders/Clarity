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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Clarity
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the attributes of a given member as a specifid type.
        /// </summary>
        /// <typeparam name="T">The type of attributes to </typeparam>
        /// <param name="member">The member to query.</param>
        /// <param name="inherit">if set to <c>true</c> searches ancestors</param>
        /// <returns>Retrieves an enumeration of matching attributes provided by the member</returns>
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
        {
            if (member == null)
            {
                //Return a default empty list if the member is not found.
                return Activator.CreateInstance<List<T>>();
            }

            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
        }

        public static string GetPropertyName<T>(this object any, Expression<Func<T>> property)
        {
            property.IfNullThrow("property");
            if (property.Body is MemberExpression)
            {
                return (property.Body as MemberExpression).Member.Name;
            }

            if (property.Body is UnaryExpression)
            {
                return ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;
            }

            //expression unhandled
            Debugger.Break();
            return null;
        }

        public static void SetProperty(this object any, string propertyName, object value)
        {
            any.IfNullThrow("any");

            var pi = any.GetType().GetProperty(propertyName);
            if (pi == null) throw new ArgumentException(string.Format("Property '{0}' not found on '{1}'", propertyName, any.GetType().Name));

            if (pi.CanWrite)
            {
                pi.SetValue(any, value, null);
            }
            else
            {
                throw new ArgumentException(string.Format("Property '{0}' is read-only", propertyName));
            }
        }

        public static T GetProperty<T>(this object any, string propertyName)
        {
            return (T)GetProperty(any, propertyName);
        }

        public static object GetProperty(this object any, string propertyName)
        {
            any.IfNullThrow("any");

            var pi = any.GetType().GetProperty(propertyName);
            if (pi == null) throw new ArgumentException(string.Format("Property '{0}' not found on '{1}'", propertyName, any.GetType().Name));

            if (pi.CanRead)
            {
                return pi.GetValue(any, null);
            }
            else
            {
                throw new ArgumentException(string.Format("Property '{0}' cannot be read", propertyName));
            }
        }

        // Summary:
        //     Adds the elements of the specified collection to the end of the System.Collections.Generic.ObservableCollection<T>.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements should be added to the end of the System.Collections.Generic.ObservableCollection<T>.
        //     The collection itself cannot be null, but it can contain elements that are
        //     null, if type T is a reference type.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.IfNullThrow("collection");
            if (items == null) return;

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// If the parameter is null throw an ArgumentNullException
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public static void IfNullThrow(this object parameter, string parameterName)
        {
            if(string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            else if (parameter.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty((string)parameter))
                    throw new ArgumentNullException(parameterName);
            }
        }
    }
}
