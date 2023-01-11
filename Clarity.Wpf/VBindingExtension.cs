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
using System.Windows;
using System.Windows.Data;

namespace Clarity.Wpf
{
    /// <summary>
    /// By default, sets the binding to validate and update on PropertyChanged
    /// </summary>
    public class VBindingExtension : Binding
    {
        public VBindingExtension()
        {
            Init();
        }

        public VBindingExtension(string path)
        {
            Init();
            Path = new PropertyPath(path);
        }

        public VBindingExtension(object source, string path)
        {
            Init();
            Source = source;
            Path = new PropertyPath(path);
        }

        private void Init()
        {
            NotifyOnValidationError = true;
            ValidatesOnDataErrors = true;
            ValidatesOnExceptions = true;
            ////UpdateSourceExceptionFilter = UpdateSourceExceptionHandler;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}
