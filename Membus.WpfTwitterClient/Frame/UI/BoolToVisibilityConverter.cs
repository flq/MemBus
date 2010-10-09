using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Membus.WpfTwitterClient.Frame.UI
{
    /// <summary>
    /// Converts a boolean to visibility. true always maps to visible. By default false maps to Collapsed, but you can override by providing a converter parameter
    /// where you pass a string that can be parsed to a member of the "Visibility" enum.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsBoolean = System.Convert.ToBoolean(value);
            if (valueAsBoolean)
                return Visibility.Visible;

            if (parameter != null)
            {
                Visibility v;
                var success = Enum.TryParse((string) parameter, out v);
                if (success) return v;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("BoolToVisibilityConverter converts only one way from a boolean model to UI visibility");
        }
    }
}