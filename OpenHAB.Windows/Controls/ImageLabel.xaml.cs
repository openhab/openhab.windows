using System;
using System.Text.RegularExpressions;
using OpenHAB.Core.Services;
using OpenHAB.Windows.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Control that represents an image with a label.
    /// </summary>
    public sealed partial class ImageLabel : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLabel"/> class.
        /// </summary>
        public ImageLabel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Bindable property for the control icon.
        /// </summary>
        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register(
            "IconPath", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), IconChangedCallback));

        private static async void IconChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImageLabel)dependencyObject;
            if (control == null)
            {
                return;
            }

            // fix IconPathState by removing empty space and special characters
            string iconPath = control.IconPath;

            Match format = Regex.Match(iconPath, @"format=svg");
            Match state = Regex.Match(iconPath, @"state=(.+?)&");
            if (state != null)
            {
                if (!string.IsNullOrEmpty(state.Value))
                {
                    string newstate = Regex.Replace(state.Groups[1].Value, "[^0-9a-zA-Z.&]", string.Empty);
                    iconPath = control.IconPath.Replace(state.Groups[1].Value, newstate, StringComparison.InvariantCulture);
                }
            }

            IIconCaching iconCaching = (IIconCaching)DIService.Instance.Services.GetService(typeof(IIconCaching));
            iconPath = await iconCaching.ResolveIconPath(iconPath, format.Success ? "svg" : "png");

            if (format.Success)
            {
                control.Icon.Source = new SvgImageSource(new Uri(iconPath));
            }
            else
            {
                control.Icon.Source = new BitmapImage(new Uri(iconPath));
            }
        }

        /// <summary>
        /// Gets or sets the IconPath.
        /// </summary>
        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value ?? string.Empty);
        }

        /// <summary>
        /// Bindable property for the label.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), TextChangedCallback));

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string LabelText
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value ?? string.Empty);
        }

        private static void TextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImageLabel)dependencyObject;

            if (control == null)
            {
                return;
            }

            control.Label.Text = control.LabelText;
        }

        /// <summary>
        /// The color for label text property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register(
         nameof(LabelForeground), typeof(SolidColorBrush), typeof(WidgetBase), new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the OpenHAB widget.
        /// </summary>
        public SolidColorBrush LabelForeground
        {
            get => (SolidColorBrush)GetValue(LabelForegroundProperty);
            set => SetValue(LabelForegroundProperty, value);
        }
    }
}
