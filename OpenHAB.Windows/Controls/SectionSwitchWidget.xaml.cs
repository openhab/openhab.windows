using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using WinRTXamlToolkit.Controls.Extensions;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch.
    /// </summary>
    public sealed partial class SectionSwitchWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionSwitchWidget"/> class.
        /// </summary>
        public SectionSwitchWidget()
        {
            InitializeComponent();
            Loaded += SectionSwitchWidget_Loaded;
        }

        private void SectionSwitchWidget_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Widget.Item?.State))
            {
                return;
            }

            SetState();
        }

        internal override void SetState()
        {
            UncheckEverything();
            var currentItem = SectionsList?.Items?.SingleOrDefault(x => ((OpenHABWidgetMapping)x).Command == Widget.Item.State);
            ContentPresenter presenter = SectionsList?.ContainerFromItem(currentItem) as ContentPresenter;

            if (presenter.GetChildren().FirstOrDefault() is ToggleButton toggleButton)
            {
                toggleButton.IsChecked = true;
            }
        }

        private void Button_OnClick(object sender, TappedRoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            if (string.CompareOrdinal(button?.Tag?.ToString(), Widget.Item.State) == 0)
            {
                SetState();
                return;
            }

            UncheckEverything();
            button.IsChecked = true;

            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, button?.Tag.ToString()));
        }

        private void UncheckEverything()
        {
            foreach (var item in SectionsList.Items)
            {
                ContentPresenter presenter = SectionsList?.ContainerFromItem(item) as ContentPresenter;
                if (presenter.GetChildren().FirstOrDefault() is ToggleButton toggleButton)
                {
                    toggleButton.IsChecked = false;
                }
            }
        }
    }
}