using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel;

namespace OpenHAB.Core.Services
{
    /// <inheritdoc/>
    public class AppManager : IAppManager
    {
        private string _startupId = "openHABStartupId";
        private ILogger<AppManager> _logger;

        /// <summary>Initializes a new instance of the <see cref="AppManager" /> class.</summary>
        public AppManager(ILogger<AppManager> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> IsStartupEnabled()
        {
            StartupTask startupTask = await StartupTask.GetAsync(_startupId);
            return startupTask.State == StartupTaskState.Enabled || startupTask.State == StartupTaskState.EnabledByPolicy;
        }

        /// <inheritdoc/>
        public async Task<bool> CanEnableAutostart()
        {
            StartupTask startupTask = await StartupTask.GetAsync(_startupId);
            return !(startupTask.State == StartupTaskState.DisabledByPolicy || startupTask.State == StartupTaskState.DisabledByUser);
        }

        /// <inheritdoc/>
        public async Task ToggleAutostart()
        {
            StartupTask startupTask = await StartupTask.GetAsync(_startupId);
            StartupTaskState newState = StartupTaskState.Disabled;

            switch (startupTask.State)
            {
                case StartupTaskState.DisabledByPolicy:
                case StartupTaskState.DisabledByUser:
                case StartupTaskState.Disabled:
                    newState = await startupTask.RequestEnableAsync();
                    _logger.LogInformation($"App autostart: {newState.ToString()}");
                    break;
                case StartupTaskState.EnabledByPolicy:
                case StartupTaskState.Enabled:
                    startupTask.Disable();
                    _logger.LogInformation($"App autostart: disabled");
                    break;
            }
        }
    }
}
