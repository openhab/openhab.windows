using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch.
    /// </summary>
    public sealed partial class SelectionWidget : WidgetBase
    {
        private System.Collections.Generic.List<SelectionMapping> selectionMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionWidget"/> class.
        /// </summary>
        public SelectionWidget()
        {
            InitializeComponent();
            Loaded += SelectionWidget_Loaded;
        }

        /// <summary>
        /// Get's called after SelectionWidget was loaded and updates the Widgets Selection Dropdown.
        /// </summary>
        private void SelectionWidget_Loaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            selectionMappings = new System.Collections.Generic.List<SelectionMapping>();
            if (Widget?.Item.CommandDescription?.CommandOptions?.Count > 0)
            {
                foreach (OpenHABCommandOptions option in Widget.Item.CommandDescription.CommandOptions)
                {
                    SelectionMapping mapping = new SelectionMapping(option.Command, option.Label);
                    selectionMappings.Add(mapping);
                }
            }

            if (Widget?.Mappings.Count > 0)
            {
                foreach (OpenHABWidgetMapping option in Widget.Mappings)
                {
                    SelectionMapping mapping = new SelectionMapping(option.Command, option.Label);
                    selectionMappings.Add(mapping);
                }
            }

            SelectionComboBox.ItemsSource = selectionMappings;
            SetState();
        }

        internal override void SetState()
        {
            SelectionMapping itemState = selectionMappings.FirstOrDefault(x => x.Command == Widget.Item.State);
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                SelectionMapping currentSelection = SelectionComboBox.SelectedItem as SelectionMapping;

                if (currentSelection != null && string.CompareOrdinal(currentSelection?.Command, itemState?.Command) != 0)
                {
                    SelectionComboBox.SelectedItem = itemState;
                }

                SelectionComboBox.SelectionChanged += Selector_OnSelectionChanged;
            });
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionMapping mapping = (SelectionMapping)e.AddedItems.FirstOrDefault();
            if (mapping == null || string.CompareOrdinal(Widget.Item.State, mapping.Command) == 0)
            {
                return;
            }

            Widget.Item.State = mapping.Command;
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, mapping.Command));
        }

        /// <summary>
        /// A Class that is used for Mapping Selectionvalues to Labels.
        /// </summary>
        private class SelectionMapping
        {
            /// <summary>
            /// Gets or sets the Command of the mapping.
            /// </summary>
            public string Command
            {
                get; set;
            }

            /// <summary>
            /// Gets or sets the Label of the mapping.
            /// </summary>
            public string Label
            {
                get; set;
            }

            /// <summary>Initializes a new instance of the <see cref="SelectionMapping" /> class.</summary>
            /// <param name="command">The command.</param>
            /// <param name="label">The label.</param>
            public SelectionMapping(string command, string label)
            {
                Command = command;
                Label = label;
            }
        }
    }
}