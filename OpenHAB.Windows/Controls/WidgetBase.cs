using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenHAB.Core.Model;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// A base class for all OpenHAB widget controls.
    /// </summary>
    public abstract class WidgetBase : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// A bindable property to bind the OpenHAB widget to the control.
        /// </summary>
        public static readonly DependencyProperty WidgetProperty = DependencyProperty.Register(
            nameof(Widget), typeof(OpenHABWidget), typeof(WidgetBase), new PropertyMetadata(default(OpenHABWidget), CommonPropertyChangedCallback));

        /// <summary>
        /// Gets or sets the OpenHAB widget.
        /// </summary>
        public OpenHABWidget Widget
        {
            get => (OpenHABWidget)GetValue(WidgetProperty);
            set => SetValue(WidgetProperty, value);
        }

        private static void CommonPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var widgetBase = dependencyObject as WidgetBase;
            widgetBase?.SetPropertyChangedHandler();
        }

        private void SetPropertyChangedHandler()
        {
            if (Widget.Item == null)
            {
                return;
            }

            Widget.Item.PropertyChanged += Item_PropertyChanged;
        }

        private async void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var widget = sender as OpenHABWidget;
            if (e.PropertyName != nameof(widget.Item.State))
            {
                return;
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, SetState);
        }

        internal abstract void SetState();

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
