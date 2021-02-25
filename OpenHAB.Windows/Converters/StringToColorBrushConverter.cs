using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace OpenHAB.Windows.Converters
{
    /// <summary>Convert color string e.g. 'green' to Color.Green. </summary>
    public class StringToColorBrushConverter : IValueConverter
    {
        /// <summary>Converts the specified value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string colorString = value as string;
            if (string.IsNullOrEmpty(colorString))
            {
                return new SolidColorBrush(Colors.Black);
            }

            Color color = (Color)XamlBindingHelper.ConvertValue(typeof(Color), colorString);
            return new SolidColorBrush(color);
        }

        /// <summary>Converts the back.</summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Color color = (Color)value;
            return color;
        }
    }
}
