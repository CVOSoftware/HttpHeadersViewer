using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HttpHeadersViewer.Helper;

namespace HttpHeadersViewer.View.Converters
{
    public class NetworkStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (string) value;
            SolidColorBrush brush = null;
            switch (status)
            {
                case NetworkHelper.ONLINE:
                    brush = new SolidColorBrush(Colors.DarkGreen);
                    break;
                case NetworkHelper.OFFLINE:
                    brush = new SolidColorBrush(Colors.Red);
                    break;
            }

            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}