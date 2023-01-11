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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Clarity
{
    public class ValidationResults : List<ValidationResult>
    {
        public static ValidationResults CreateFromAttributes(object entity)
        {
            entity.IfNullThrow("entity");

            var results = new ValidationResults();
            ValidationContext context = new ValidationContext(entity, null, null);

            foreach (var p in entity.GetType().GetProperties())
            {
                var attrs = (ValidationAttribute[])GetAttributes<ValidationAttribute>(p, false).ToArray();
                foreach (var attr in attrs)
                {
                    var value = p.GetValue(entity, null);
                    context.MemberName = p.Name;
                    ValidationResult validationResult = null;
                    bool isInValid = false;
                    string attributeErrorName = attr.ErrorMessageResourceName;

                    try
                    {
                        if (attr.ErrorMessage == null)
                        {
                            attr.ErrorMessage = "Ignore";
                        }

                        attr.ErrorMessageResourceName = null;
                        validationResult = attr.GetValidationResult(value, context);
                        isInValid = validationResult != null;
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (ex.Message.StartsWith("Both ErrorMessageResourceType"))
                        {
                            isInValid = true;
                        }
                        else
                        {
                            //                            Logger.LogException(ex);
                        }
                    }

                    attr.ErrorMessageResourceName = attributeErrorName;

                    if (isInValid)
                    {
                        {
                            if (string.IsNullOrEmpty(attr.ErrorMessageResourceName))
                            {
                                var result = new ValidationResult(attr.ErrorMessage, new List<string>() { p.Name });
                                results.Add(result);
                            }
                            else
                            {
                                var result = new ValidationResult(attr.ErrorMessageResourceName, new List<string>() { p.Name });
                                results.Add(result);
                            }
                        }
                    }
                }
            }

            return results;
        }

        private static IEnumerable<T> GetAttributes<T>(MemberInfo member, bool inherit)
        {
            if (member == null)
            {
                //Return a default empty list if the member is not found.
                return Activator.CreateInstance<List<T>>();
            }

            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
        }

        public void Add<T>(string errorMessage, Expression<Func<T>> property)
        {
            errorMessage.IfNullThrow("errorMessage");
            property.IfNullThrow("property");

            string[] members = new[] { (property.Body as MemberExpression).Member.Name };

            var result = new ValidationResult(errorMessage, members);

            Add(result);
        }

        public bool IsValid
        {
            get
            {
                return Count == 0;
            }
        }

        /// <summary>
        /// Returns an error message for a given property
        /// </summary>
        /// <param name="propertyName">Name of the property to check</param>
        /// <returns>A populated string if there is an error, otherwise String.Empty</returns>
        public string GetValidationError(string propertyName)
        {
            foreach (var result in this)
            {
                if (result.MemberNames.Contains(propertyName))
                {
                    return result.ErrorMessage;
                }
            }

            return string.Empty;
        }
    }
}
