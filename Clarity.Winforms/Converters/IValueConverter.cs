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

namespace Clarity.Winforms.Converters
{
    public interface IValueConverter
    {
        object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
    }
}
