using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using OpenHAB.Core.Common;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Services;
using Windows.ApplicationModel;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for user defined settings.
    /// </summary>
    public class SettingsViewModel : ViewModelBase<object>
    {
        private readonly IIconCaching _iconCaching;
        private readonly ILogger<SettingsViewModel> _logger;
        private ActionCommand _clearIconCacheCommand;
        private ConfigurationViewModel _configuration;
        private ActionCommand _saveCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel(ConfigurationViewModel configurationViewModel, IIconCaching iconCaching, ILogger<SettingsViewModel> logger)
            : base(new object())
        {
            _configuration = configurationViewModel;
            _configuration.PropertyChanged += Configuration_PropertyChanged;

            _iconCaching = iconCaching;

            _logger = logger;
        }

        /// <summary>
        /// Gets the clear icon cache command to persist the settings.
        /// </summary>
        /// <value>The save command.</value>
        public ActionCommand ClearIconCacheCommand => _clearIconCacheCommand ?? (_clearIconCacheCommand = new ActionCommand(ClearIcons, CanClearIcons));

        /// <summary>
        /// Gets the save command to persist the settings.
        /// </summary>
        /// <value>The save command.</value>
        public ActionCommand SaveCommand => _saveCommand ?? (_saveCommand = new ActionCommand(PersistSettings, CanPersistSettings));

        /// <summary>
        /// Gets or sets the current user-defined settings.
        /// </summary>
        public ConfigurationViewModel Settings
        {
            get => _configuration;
            set => Set(ref _configuration, value);
        }

        /// <summary>
        /// Gets the application version number.
        /// </summary>
        /// <value>The version number.</value>
        public string Version
        {
            get
            {
                Version version = new Version(
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);

                return version.ToString();
            }
        }

        /// <summary>
        /// Save the user defined settings to the UWP settings storage.
        /// </summary>
        public void PersistSettings(object obj)
        {
            _logger.LogInformation("Execute save settings command");

            bool validConnectionConfig = CheckForValidConnectionConfig();
            if (validConnectionConfig)
            {
                bool savedSuccessful = _configuration.Save();
                Messenger.Default.Send(new SettingsUpdatedMessage(validConnectionConfig, savedSuccessful));
            }
        }

        private bool CheckForValidConnectionConfig()
        {
            return _configuration.IsConnectionConfigValid() ||
               (_configuration.IsRunningInDemoMode.HasValue && _configuration.IsRunningInDemoMode.Value);
        }

        private bool CanClearIcons(object arg)
        {
            return true;
        }

        private bool CanPersistSettings(object arg)
        {
            bool validConnectionConfig = CheckForValidConnectionConfig();
            Messenger.Default.Send(new SettingsValidationMessage(validConnectionConfig));

            return validConnectionConfig && _configuration.IsDirty;
        }

        private void ClearIcons(object obj)
        {
            _iconCaching.ClearIconCache();
        }

        private void Configuration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveCommand.InvokeCanExecuteChanged(null);
        }
    }
}