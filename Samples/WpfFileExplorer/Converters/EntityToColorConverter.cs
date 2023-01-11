using ExplorerLib.Entities;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfFileExplorer.Converters
{
    public class EntityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Folder)
                return Colors.Blue.ToString();

            return Colors.Black.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
