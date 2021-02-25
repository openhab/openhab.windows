using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Services.Store.Engagement;
using OpenHAB.Core;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Core.Services;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for starting the application.
    /// </summary>
    public class MainViewModel : ViewModelBase<object>
    {
        private readonly StoreServicesFeedbackLauncher _feedbackLauncher;
        private readonly IOpenHAB _openHabsdk;
        private readonly ISettingsService _settingsService;
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<OpenHABWidget> _currentWidgets;
        private ActionCommand _feedbackCommand;
        private bool _isDataLoading;
        private ILogger<MainViewModel> _logger;
        private ActionCommand _refreshCommand;
        private SitemapViewModel _selectedSitemap;
        private OpenHABWidget _selectedWidget;
        private ObservableCollection<SitemapViewModel> _sitemaps;
        private string _subtitle;
        private OpenHABVersion _version;
        private object _selectedMenuItem;
        private ActionCommand _reloadSitemapCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="openHabsdk">The OpenHAB SDK object.</param>
        /// <param name="settingsService">Setting service instance.</param>
        /// <param name="logger">Logger class instance.</param>
        public MainViewModel(IOpenHAB openHabsdk, ISettingsService settingsService, ILogger<MainViewModel> logger)
            : base(new object())
        {
            _logger = logger;
            CurrentWidgets = new ObservableCollection<OpenHABWidget>();

            _openHabsdk = openHabsdk;
            _settingsService = settingsService;
            _feedbackLauncher = StoreServicesFeedbackLauncher.GetDefault();
            _cancellationTokenSource = new CancellationTokenSource();

            Messenger.Default.Register<TriggerCommandMessage>(this, async msg => await TriggerCommand(msg).ConfigureAwait(false));
            Messenger.Default.Register<WidgetClickedMessage>(this, msg => OnWidgetClickedAsync(msg.Widget));
        }

        /// <summary>
        /// Gets or sets the widgets currently on screen.
        /// </summary>
        public ObservableCollection<OpenHABWidget> CurrentWidgets
        {
            get => _currentWidgets;
            set => Set(ref _currentWidgets, value);
        }

        /// <summary>Gets the command to open feedback application.</summary>
        /// <value>The feedback command.</value>
        public ActionCommand FeedbackCommand => _feedbackCommand ?? (_feedbackCommand = new ActionCommand(ExecuteFeedbackCommand, CanExecuteFeedbackCommand));

        /// <summary>
        /// Gets or sets a value indicating whether data is loaded from an OpenHAB instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if data will loaded; otherwise, <c>false</c>.</value>
        public bool IsDataLoading
        {
            get => _isDataLoading;
            set => Set(ref _isDataLoading, value);
        }

        /// <summary>Gets the command to refresh sitemap and widget data.</summary>
        /// <value>The refresh command.</value>
        public ActionCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new ActionCommand(ExecuteRefreshCommandAsync, CanExecuteRefreshCommand));

        /// <summary>Gets the command to reload sitemap and widget data.</summary>
        /// <value>The refresh command.</value>
        public ActionCommand ReloadSitemapCommand => _reloadSitemapCommand ?? (_reloadSitemapCommand = new ActionCommand(ExecuteReloadSitemapCommand, CanExecuteReloadSitemapCommand));

        /// <summary>
        /// Gets or sets the sitemap currently selected by the user.
        /// </summary>
        public SitemapViewModel SelectedSitemap
        {
            get
            {
                return _selectedSitemap;
            }

            set
            {
                if (Set(ref _selectedSitemap, value))
                {
                    if (_selectedSitemap != null)
                    {
                        _settingsService.SaveCurrentSitemap(_selectedSitemap.Name);
                        Subtitle = _selectedSitemap.Label;
                    }

                    if (!_isDataLoading && (_selectedSitemap?.Widgets == null || _selectedSitemap?.Widgets.Count == 0))
                    {
                        CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                        dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            LoadWidgets();
                            WidgetNavigationService.ClearWidgetNavigation();
                        });
                    }
                    else
                    {
                        SetWidgetsOnScreen(SelectedSitemap.Widgets);
                    }

                    SelectedMenuItem = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected menu item.
        /// </summary>
        public object SelectedMenuItem
        {
            get
            {
                return _selectedMenuItem;
            }

            set
            {
                SitemapViewModel sitemapViewModel = value as SitemapViewModel;
                if (sitemapViewModel != null && SelectedSitemap != value)
                {
                    SelectedSitemap = sitemapViewModel;
                }

                Set(ref _selectedMenuItem, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected widget.
        /// </summary>
        public OpenHABWidget SelectedWidget
        {
            get => _selectedWidget;
            set => Set(ref _selectedWidget, value);
        }

        /// <summary>
        /// Gets or sets a collection of OpenHAB sitemaps.
        /// </summary>
        public ObservableCollection<SitemapViewModel> Sitemaps
        {
            get => _sitemaps;
            set => Set(ref _sitemaps, value);
        }

        /// <summary>
        /// Gets or sets the subtitle of the page.
        /// </summary>
        public string Subtitle
        {
            get => _subtitle;
            set => Set(ref _subtitle, value);
        }

        #region Commands

        private bool CanExecuteReloadSitemapCommand(object arg)
        {
            return !IsDataLoading;
        }

        private bool CanExecuteFeedbackCommand(object obj)
        {
            return StoreServicesFeedbackLauncher.IsSupported();
        }

        private bool CanExecuteRefreshCommand(object arg)
        {
            return !IsDataLoading;
        }

        private async void ExecuteFeedbackCommand(object obj)
        {
            await _feedbackLauncher.LaunchAsync();
        }

        private async void ExecuteRefreshCommandAsync(object obj)
        {
            await LoadSitemapsAndItemData().ConfigureAwait(false);
        }

        private async void ExecuteReloadSitemapCommand(object obj)
        {
            await ReloadSitemap().ConfigureAwait(false);
        }

        private async Task TriggerCommand(TriggerCommandMessage message)
        {
            HttpResponseResult<bool> result = await _openHabsdk.SendCommand(message.Item, message.Command).ConfigureAwait(false);
            if (!result.Content)
            {
                string errorMessage = AppResources.Errors.GetString("CommandFailed");
                errorMessage = string.Format(errorMessage, message.Command, message.Item?.Name);

                Messenger.Default.Send<FireErrorMessage>(new FireErrorMessage(errorMessage));
            }
        }

        #endregion

        #region Load sitemap and Data

        /// <summary>
        /// Loads the sitemap data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task LoadSitemapsAndItemData()
        {
            await LoadData(_cancellationTokenSource.Token).ConfigureAwait(false);
        }

        private async Task ReloadSitemap()
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurrentWidgets?.Clear();
                IsDataLoading = true;

                if (SelectedWidget != null)
                {
                    await LoadWidgets().ConfigureAwait(false);
                    OpenHABWidget widget = FindWidget(SelectedWidget.WidgetId, SelectedSitemap.Widgets);
                    if (widget != null)
                    {
                        await OnWidgetClickedAsync(widget);
                    }
                    else
                    {
                        SelectedWidget = null;
                        WidgetNavigationService.ClearWidgetNavigation();
                    }
                }
                else
                {
                    await LoadWidgets().ConfigureAwait(false);
                }

                IsDataLoading = false;
                ReloadSitemapCommand.InvokeCanExecuteChanged(null);
                RefreshCommand.InvokeCanExecuteChanged(null);
            });
        }

        private async void CancelSyncCallbackAsync()
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Sitemaps?.Clear();
                CurrentWidgets?.Clear();
                Subtitle = null;
                IsDataLoading = false;
            });
        }

        private async Task LoadData(CancellationToken cancellationToken)
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            cancellationToken.Register(CancelSyncCallbackAsync);

            try
            {
                if (IsDataLoading)
                {
                    return;
                }

                _logger.LogInformation("Load available sitemap's and their items");

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsDataLoading = true;
                    Sitemaps?.Clear();
                    CurrentWidgets?.Clear();
                    Subtitle = null;
                });

                Settings settings = _settingsService.Load();
                if (settings.LocalConnection == null && settings.RemoteConnection == null &&
                    (!settings.IsRunningInDemoMode.HasValue || !settings.IsRunningInDemoMode.Value))
                {
                    Messenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                }

                bool isSuccessful = await _openHabsdk.ResetConnection().ConfigureAwait(false);
                if (!isSuccessful)
                {
                    Messenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                    return;
                }

                _version = await _openHabsdk.GetOpenHABVersion().ConfigureAwait(false);
                if (_version == OpenHABVersion.None)
                {
                    Messenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                    return;
                }

                _settingsService.ServerVersion = _version;

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                List<SitemapViewModel> sitemapViewModels = await LoadSitemaps(settings).ConfigureAwait(false);

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Sitemaps = new ObservableCollection<SitemapViewModel>(sitemapViewModels);
                    _openHabsdk.StartItemUpdates(cancellationToken);

                    OpenLastOrDefaultSitemap();

                    if (SelectedWidget != null)
                    {
                        await LoadWidgets().ConfigureAwait(false);
                        OpenHABWidget widget = FindWidget(SelectedWidget.WidgetId, SelectedSitemap.Widgets);
                        if (widget != null)
                        {
                            await OnWidgetClickedAsync(widget);
                        }
                        else
                        {
                            SelectedWidget = null;
                            WidgetNavigationService.ClearWidgetNavigation();
                        }
                    }
                    else
                    {
                        await LoadWidgets().ConfigureAwait(false);
                    }

                    IsDataLoading = false;
                    ReloadSitemapCommand.InvokeCanExecuteChanged(null);
                    RefreshCommand.InvokeCanExecuteChanged(null);
                });
            }
            catch (OpenHABException ex)
            {
                _logger.LogError(ex, "Load data failed.");
                Messenger.Default.Send(new FireErrorMessage(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load data failed.");
            }
            finally
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsDataLoading = false;
                });
            }
        }

        private async Task<List<SitemapViewModel>> LoadSitemaps(Settings settings)
        {
            Func<OpenHABSitemap, bool> defaultSitemapFilter = (sitemap) =>
            {
                return !sitemap.Name.Equals("_default", StringComparison.InvariantCultureIgnoreCase);
            };

            List<Func<OpenHABSitemap, bool>> filters = new List<Func<OpenHABSitemap, bool>>();

            if (!settings.ShowDefaultSitemap)
            {
                filters.Add(defaultSitemapFilter);
            }

            ICollection<OpenHABSitemap> sitemaps = await _openHabsdk.LoadSiteMaps(_version, filters).ConfigureAwait(false);
            List<SitemapViewModel> sitemapViewModels = new List<SitemapViewModel>();
            sitemaps.ToList().ForEach(x => sitemapViewModels.Add(new SitemapViewModel(x)));

            return sitemapViewModels;
        }

        private void OpenLastOrDefaultSitemap()
        {
            string sitemapName = _settingsService.LoadLastSitemap();

            if (string.IsNullOrWhiteSpace(sitemapName))
            {
                _logger.LogInformation("No sitemap was selected in the past -> Pick first entry from list");

                SelectedSitemap = Sitemaps.FirstOrDefault();
                return;
            }

            SelectedSitemap = Sitemaps.FirstOrDefault(x => x.Name == sitemapName);
            if (SelectedSitemap == null)
            {
                _logger.LogInformation($"Unable to find sitemap '{sitemapName}' -> Pick first entry from list");
                SelectedSitemap = Sitemaps.FirstOrDefault();
            }

            Subtitle = SelectedSitemap.Label;
        }

        #endregion

        #region Widget Handing

        /// <summary>
        /// Navigate backwards between linked pages.
        /// </summary>
        public void WidgetGoBack()
        {
            OpenHABWidget widget = WidgetNavigationService.GoBack();

            if (SelectedSitemap == null)
            {
                return;
            }

            Subtitle = widget == null ? SelectedSitemap?.Label : widget.Label;
            SetWidgetsOnScreen(widget != null ? widget.LinkedPage.Widgets : SelectedSitemap.Widgets);
        }

        private async Task LoadWidgets()
        {
            if (SelectedSitemap == null)
            {
                return;
            }

            CurrentWidgets.Clear();
            IsDataLoading = true;

            await SelectedSitemap.LoadWidgets(_version).ConfigureAwait(false);

            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetWidgetsOnScreen(SelectedSitemap.Widgets);
                IsDataLoading = false;
            });
        }

        private async Task OnWidgetClickedAsync(OpenHABWidget widget)
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SelectedWidget = widget;
                if (SelectedWidget.LinkedPage == null || !SelectedWidget.LinkedPage.Widgets.Any())
                {
                    return;
                }

                Subtitle = SelectedWidget.Label;

                WidgetNavigationService.Navigate(SelectedWidget);
                SetWidgetsOnScreen(SelectedWidget?.LinkedPage?.Widgets);
            });
        }

        private void SetWidgetsOnScreen(ICollection<OpenHABWidget> widgets)
        {
            CurrentWidgets.Clear();
            CurrentWidgets.AddRange(widgets);
        }

        private OpenHABWidget FindWidget(string widgetId, ICollection<OpenHABWidget> widgets)
        {
            OpenHABWidget openHABWidget = null;
            if (widgets == null || widgets.Count == 0)
            {
                return openHABWidget;
            }

            foreach (OpenHABWidget widget in widgets)
            {
                if (string.CompareOrdinal(widget.WidgetId, widgetId) == 0)
                {
                    return widget;
                }

                ICollection<OpenHABWidget> childWidgets = widget.Type.CompareTo("Group") == 0 ? widget.LinkedPage?.Widgets : widget.Children;
                openHABWidget = FindWidget(widgetId, childWidgets);
                if (openHABWidget != null)
                {
                    return openHABWidget;
                }
            }

            return openHABWidget;
        }

        #endregion
    }
}