using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Notifications;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Notifications;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace OpenHAB.Core.Services
{
    /// <inheritdoc/>
    public class NotificationManager : INotificationManager
    {
        private IItemManager _itemManager;
        private string _iconFormat;
        private ISettingsService _settingsService;
        private int _notficationCounter;

        /// <summary>Initializes a new instance of the <see cref="NotificationManager" /> class.</summary>
        /// <param name="itemStateManager">The item state manager.</param>
        /// <param name="settingsService">Setting service.</param>
        /// <param name="settings">Application Settings.</param>
        public NotificationManager(IItemManager itemStateManager, ISettingsService settingsService, Settings settings)
        {
            Messenger.Default.Register<ItemStateChangedMessage>(this, HandleUpdateItemMessage);
            _itemManager = itemStateManager;
            _iconFormat = settings.UseSVGIcons ? "svg" : "png";
            _settingsService = settingsService;
            _notficationCounter = 0;
        }

        private void HandleUpdateItemMessage(ItemStateChangedMessage obj)
        {
            Settings settings = _settingsService.Load();

            _notficationCounter++;

            string itemName = obj.ItemName;
            string itemImage = string.Empty;
            if (_itemManager.TryGetItem(obj.ItemName, out OpenHABItem item))
            {
                itemName = item.Label;
                string state = item?.State ?? "ON";
                itemImage = $"{OpenHABHttpClient.BaseUrl}icon/{item.Category}?state={state}&format={_iconFormat}";
            }

            TriggerTileNotificationForItem(itemName, itemImage, obj.Value, obj.OldValue);

            if (settings.NotificationsEnable.HasValue && !settings.NotificationsEnable.Value)
            {
                return;
            }

            TriggerToastNotificationForItem(itemName, itemImage, obj.Value, obj.OldValue);
            SetBadgeNumber(_notficationCounter);
        }

        #region Toast Notification

        private void TriggerToastNotificationForItem(string itemName, string itemImage, string value, string oldValue)
        {
            string message = GetMessage(itemName, value, oldValue, "NotificationToast", "NotificationToastSimple");

            var notifier = ToastNotificationManager.CreateToastNotifier();
            var xmdock = CreateToastMessage(message, itemImage);
            var toast = new ToastNotification(xmdock);

            notifier.Show(toast);
        }

        private XmlDocument CreateToastMessage(string message, string image)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "openHAB for Windows",
                                HintMaxLines = 1,
                            },
                            new AdaptiveText()
                            {
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Show Item", "action=show")
                        {
                            ActivationType = ToastActivationType.Foreground,
                        }
                    }
                }
            };

            return toastContent.GetXml();
        }

        #endregion

        #region Tile Notification

        private void TriggerTileNotificationForItem(string itemName, string itemImage, string value, string oldValue)
        {
            string message = GetMessage(itemName, value, oldValue, "NotificationTile", "NotificationTileSimple");

            TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            XmlDocument tileNotifcationXML = CreateTileNotificaiton(itemName, message, value, itemImage);
            TileNotification tileNotif = new TileNotification(tileNotifcationXML);

            tileUpdater.Update(tileNotif);
        }

        private XmlDocument CreateTileNotificaiton(string itemName, string message, string value, string itemImage)
        {
            var tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo,
                    TileSmall = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = itemName,
                        HintStyle = AdaptiveTextStyle.Body,
                        HintAlign = AdaptiveTextAlign.Center
                    },
                    new AdaptiveText()
                    {
                        Text = value,
                        HintStyle = AdaptiveTextStyle.Base,
                        HintAlign = AdaptiveTextAlign.Center
                    }
                }
                        }
                    },
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = itemName,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = message,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true,
                                    HintMaxLines = 3
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 33,
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = itemImage
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = itemName,
                                                    HintStyle = AdaptiveTextStyle.Caption
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = message,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintWrap = true,
                                                    HintMaxLines = 3
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 2,
                                            Children =
                                            {
                                                new AdaptiveImage()
                                                {
                                                    Source = itemImage
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1
                                        }
                                    }
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Empty
                                },
                                new AdaptiveText()
                                {
                                    Text = itemName,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = message,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true,
                                    HintMaxLines = 3
                                }
                            }
                        }
                    }
                }
            };

            return tileContent.GetXml();
        }

        #endregion

        #region Badge Notification

        /// <inheritdoc/>
        public void ResetBadgeCount()
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
        }

        private void SetBadgeNumber(int num)
        {
            // Get the blank badge XML payload for a badge number
            XmlDocument badgeXml =
                BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            // Set the value of the badge in the XML to our number
            Windows.Data.Xml.Dom.XmlElement badgeElement = badgeXml.SelectSingleNode("/badge") as Windows.Data.Xml.Dom.XmlElement;
            badgeElement.SetAttribute("value", num.ToString());

            // Create the badge notification
            BadgeNotification badge = new BadgeNotification(badgeXml);

            // Create the badge updater for the application
            BadgeUpdater badgeUpdater =
                BadgeUpdateManager.CreateBadgeUpdaterForApplication();

            // And update the badge
            badgeUpdater.Update(badge);
        }

        #endregion

        private string GetMessage(
            string itemName,
            string itemValue,
            string oldItemValue,
            string valueChangedMessageRessource,
            string stateChangedMessageRessource)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(oldItemValue))
            {
                message = AppResources.Values.GetString(valueChangedMessageRessource);
            }
            else
            {
                message = AppResources.Values.GetString(stateChangedMessageRessource);
            }

            message = string.Format(CultureInfo.InvariantCulture, message, itemName, oldItemValue, itemValue);

            return message;
        }
    }
}
