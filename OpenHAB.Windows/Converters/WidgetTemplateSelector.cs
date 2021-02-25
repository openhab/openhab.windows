using System.Linq;
using OpenHAB.Core.Common;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// TemplateSelector that determines what widget needs to be shown in the UI.
    /// </summary>
    public class WidgetTemplateSelector : DataTemplateSelector
    {
        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var widget = item as OpenHABWidget;
            var uiElement = container as UIElement;

            if (widget == null)
            {
                return null;
            }

            var itemType = GetItemViewType(widget);
            switch (itemType)
            {
                case WidgetTypeEnum.ColorPicker:
                    return ColorTemplate;
                case WidgetTypeEnum.Group:
                    return PageLinkTemplate;
                case WidgetTypeEnum.Frame:
                    VariableSizedWrapGrid.SetRowSpan(uiElement, widget.Children.Count + 1);
                    return FrameTemplate;
                case WidgetTypeEnum.Switch:
                    return SwitchTemplate;
                case WidgetTypeEnum.SectionSwitch:
                    return SectionSwitchTemplate;
                case WidgetTypeEnum.RollerShutter:
                    return RollershutterTemplate;
                case WidgetTypeEnum.Slider:
                    return SliderTemplate;
                case WidgetTypeEnum.DateTime:
                case WidgetTypeEnum.Text:
                    return TextTemplate;
                case WidgetTypeEnum.Image:
                    return ImageTemplate;
                case WidgetTypeEnum.Selection:
                    return SelectionTemplate;
                case WidgetTypeEnum.Setpoint:
                    return SetpointTemplate;
                case WidgetTypeEnum.Chart:
                    return ChartTemplate;
                case WidgetTypeEnum.Video:
                case WidgetTypeEnum.VideoMjpeg:
                    return MjpegTemplate;
                case WidgetTypeEnum.Mapview:
                    return MapViewTemplate;
                case WidgetTypeEnum.Webview:
                    return WebViewTemplate;
                default:
                    return FrameTemplate;
            }
        }

        /// <summary>
        /// Gets or sets the template for a Frame control.
        /// </summary>
        public DataTemplate FrameTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Selection control.
        /// </summary>
        public DataTemplate SelectionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a MJPEG video control.
        /// </summary>
        public DataTemplate MjpegTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a rollershutter control.
        /// </summary>
        public DataTemplate RollershutterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Pagelink control.
        /// </summary>
        public DataTemplate PageLinkTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Switch control.
        /// </summary>
        public DataTemplate SwitchTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Slider control.
        /// </summary>
        public DataTemplate SliderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Text control.
        /// </summary>
        public DataTemplate TextTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Image control.
        /// </summary>
        public DataTemplate ImageTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a section switch control.
        /// </summary>
        public DataTemplate SectionSwitchTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a color picker control.
        /// </summary>
        public DataTemplate ColorTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a setpoint control.
        /// </summary>
        public DataTemplate SetpointTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a chart control.
        /// </summary>
        public DataTemplate ChartTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a map view control.
        /// </summary>
        public DataTemplate MapViewTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a web view control.
        /// </summary>
        public DataTemplate WebViewTemplate { get; set; }

        private WidgetTypeEnum GetItemViewType(OpenHABWidget openHABWidget)
        {
            if (openHABWidget.Type.Equals("Switch"))
            {
                if (openHABWidget.Mappings != null && openHABWidget.Mappings.Any())
                {
                    return WidgetTypeEnum.SectionSwitch;
                }

                if (openHABWidget.Item != null)
                {
                    if (openHABWidget.Item.Type != null)
                    {
                        // RollerShutterItem changed to RollerShutter in later builds of OH2
                        if ("RollershutterItem".Equals(openHABWidget.Item.Type) ||
                            "Rollershutter".Equals(openHABWidget.Item.Type) ||
                            "Rollershutter".Equals(openHABWidget.Item.GroupType))
                        {
                            return WidgetTypeEnum.RollerShutter;
                        }

                        return WidgetTypeEnum.Switch;
                    }

                    return WidgetTypeEnum.Switch;
                }

                return WidgetTypeEnum.Switch;
            }

            if (openHABWidget.Type.Equals("Video"))
            {
                if (openHABWidget.Encoding != null)
                {
                    if (openHABWidget.Encoding.Equals("mjpeg"))
                    {
                        return WidgetTypeEnum.VideoMjpeg;
                    }

                    return WidgetTypeEnum.Video;
                }

                return WidgetTypeEnum.Video;
            }

            try
            {
                return Enum<WidgetTypeEnum>.Parse(openHABWidget.Type);
            }
            catch (System.Exception ex)
            {
                return WidgetTypeEnum.Generic;
            }
        }
    }
}