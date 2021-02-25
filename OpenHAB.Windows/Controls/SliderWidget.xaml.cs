using System;
using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB slider.
    /// </summary>
    public sealed partial class SliderWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderWidget"/> class.
        /// </summary>
        public SliderWidget()
        {
            InitializeComponent();
        }

        private void RadialSlider_OnValueChanged(object sender, EventArgs e)
        {
            if (Widget == null)
            {
                return;
            }

            Widget.Item.UpdateValue(((RadialSlider)sender)?.Value);
            RaisePropertyChanged(nameof(Widget));
        }

        private void Widget_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            double maxval = Widget.MaxValue;
            double minval = Widget.MinValue;
            if (maxval == minval)
            {
                // use default
                maxval = 100;
                minval = 0;
            }

            if (Widget.Item.GetStateAsDoubleValue() <= minval)
            {
                Widget.Item.UpdateValue(maxval);
            }
            else
            {
                Widget.Item.UpdateValue(minval);
            }

            RaisePropertyChanged(nameof(Widget));
        }

        internal override void SetState()
        {
        }

        private void SliderWidget_Loaded(object sender, RoutedEventArgs e)
        {
            if (Widget.MaxValue != 0)
            {
                radialSlider.Maximum = Widget.MaxValue;
            }

            if (Widget.MinValue != 0)
            {
                radialSlider.Minimum = Widget.MinValue;
            }
        }
    }
}
