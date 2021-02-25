using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Class to convert null to boolean.
    /// </summary>
    public class NullToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Visible for true, otherwise collapsed.</returns>
        /// <exception cref="System.InvalidOperationException">The target must be a Visibility.</exception>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a Boolean");
            }

            return !(value == null);
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Visibility.Visible => true and Visibility.Collapsed => false.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
