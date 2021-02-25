using System;
using System.Text.RegularExpressions;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using OpenHAB.Windows.Services;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Converts an OpenHAB widget icon to a full path.
    /// </summary>
    public class IconToPathConverter : IValueConverter
    {
        private Settings _settings;

        /// <summary>Initializes a new instance of the <see cref="IconToPathConverter" /> class.</summary>
        public IconToPathConverter()
        {
            _settings = (Settings)DIService.Instance.Services.GetService(typeof(Settings));
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settingsService = (ISettingsService)DIService.Instance.Services.GetService(typeof(ISettingsService));
            OpenHABVersion openHABVersion = settingsService.ServerVersion;

            var serverUrl = OpenHABHttpClient.BaseUrl;

            var widget = value as OpenHABWidget;
            var state = widget.Item?.State ?? "ON";
            string iconFormat = _settings.UseSVGIcons ? "svg" : "png";

            var regMatch = Regex.Match(state, @"\d+");
            if (regMatch.Success)
            {
                state = regMatch.Value;
            }

            return openHABVersion == OpenHABVersion.Two || openHABVersion == OpenHABVersion.Three ?
                $"{serverUrl}icon/{widget.Icon}?state={state}&format={iconFormat}" :
                $"{serverUrl}images/{widget.Icon}.png";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
