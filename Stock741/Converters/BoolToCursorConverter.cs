using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Stock741.Converters
{
    public class BoolToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isBusy && isBusy ? Cursors.Wait : Cursors.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
