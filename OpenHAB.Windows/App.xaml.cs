using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using OpenHAB.Core.Services;
using OpenHAB.Windows.Services;
using OpenHAB.Windows.View;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OpenHAB.Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private ILogger<App> _logger;
        private ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            _logger = (ILogger<App>)DIService.Instance.Services.GetService(typeof(ILogger<App>));
            _settingsService = (ISettingsService)DIService.Instance.Services.GetService(typeof(ISettingsService));

            Suspending += OnSuspending;
            UnhandledException += App_UnhandledException;

            INotificationManager notificationManager = (INotificationManager)DIService.Instance.Services.GetService(typeof(INotificationManager));
            notificationManager.ResetBadgeCount();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            _logger.LogInformation("=== Start Application ===");

            _settingsService.SetProgramLanguage(null);

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();

                Settings settings = _settingsService.Load();

                if (settings.StartAppMinimized.HasValue && settings.StartAppMinimized.Value)
                {
                    IList<AppDiagnosticInfo> infos = await AppDiagnosticInfo.RequestInfoForAppAsync();
                    AppDiagnosticInfo appDiagnosticInfo = infos.FirstOrDefault();

                    if (appDiagnosticInfo != null)
                    {
                        IList<AppResourceGroupInfo> resourceInfos = appDiagnosticInfo.GetResourceGroups();
                        await resourceInfos[0].StartSuspendAsync();
                    }
                }
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            _logger.LogInformation("=== Close Application ===");

            deferral.Complete();
        }

        /// <summary>Handles the UnhandledException event of the App control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="global::Windows.UI.Xaml.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void App_UnhandledException(object sender, global::Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _logger.LogCritical(e.Exception, "UnhandledException occurred");
        }
    }
}
