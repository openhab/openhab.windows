using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Factory class for OpenHAB widgets.
    /// </summary>
    public class OpenHABWidgetFactory
    {
        public static OpenHABWidget Parse(XElement startNode)
        {
            return ParseNode(startNode);
        }

        private static OpenHABWidget ParseNode(XElement startNode)
        {
            if (!startNode.HasElements)
            {
                return null;
            }

            OpenHABWidget widget = new OpenHABWidget()
            {
                WidgetId = startNode.Element("widgetId")?.Value,
                Type = startNode.Element("type")?.Value,
                Label = startNode.Element("label")?.Value,
                State = startNode.Element("state")?.Value,
                Icon = startNode.Element("icon")?.Value,
                Url = startNode.Element("url")?.Value,
                Children = new List<OpenHABWidget>(),
            };

            XElement linkedPage = startNode.Element("linkedPage");
            if (linkedPage != null)
            {
                ParseLinkedPage(linkedPage, widget);
            }

            ParseItem(startNode.Element("item"), widget);
            ParseChildren(startNode, widget);
            ParseMappings(startNode, widget);

            return widget;
        }

        private static void ParseMappings(XElement startNode, OpenHABWidget widget)
        {
            widget.Mappings = new List<OpenHABWidgetMapping>();

            foreach (XElement childNode in startNode.Elements("mapping"))
            {
                string command = childNode.Element("command")?.Value;
                string label = childNode.Element("label")?.Value;
                widget.Mappings.Add(new OpenHABWidgetMapping(command, label));
            }
        }

        private static void ParseLinkedPage(XElement linkedPage, OpenHABWidget parentWidget)
        {
            parentWidget.LinkedPage = new OpenHABSitemap(linkedPage) { Widgets = new List<OpenHABWidget>() };
            foreach (XElement childNode in linkedPage.Elements("widget"))
            {
                var widget = Parse(childNode);
                widget.Parent = parentWidget;
                widget.LinkedPage.Widgets.Add(widget);
            }
        }

        private static void ParseChildren(XElement startNode, OpenHABWidget parentWidget)
        {
            foreach (XElement childNode in startNode.Elements("widget"))
            {
                var widget = Parse(childNode);
                widget.Parent = parentWidget;
                widget.Children.Add(widget);

                XElement linkedPage = childNode.Element("linkedPage");

                if (linkedPage != null)
                {
                    ParseLinkedPage(linkedPage, widget);
                }
            }
        }

        private static void ParseItem(XElement element, OpenHABWidget widget)
        {
            if (element == null)
            {
                return;
            }

            widget.Item = new OpenHABItem(element);
        }
    }
}
