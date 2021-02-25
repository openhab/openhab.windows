namespace OpenHAB.Core.Messages
{
    /// <summary>
    /// A message object that triggers a refresh of data after changing the settings.
    /// </summary>
    public class SettingsUpdatedMessage
    {
        /// <summary>Initializes a new instance of the <see cref="SettingsUpdatedMessage"/> class.</summary>
        /// <param name="isSettingsValid">if set to <c>true</c> [is settings valid].</param>
        /// <param name="settingsPersisted">Indicates settings changes are persisted.</param>
        public SettingsUpdatedMessage(bool isSettingsValid, bool settingsPersisted)
        {
            SettingsPersisted = settingsPersisted;
            IsSettingsValid = isSettingsValid;
        }

        /// <summary>Gets a value indicating whether settings update valid.</summary>
        /// <value>
        ///   <c>true</c> if this settings changes are valid; otherwise, <c>false</c>.</value>
        public bool IsSettingsValid
        {
            get;
        }

        /// <summary>Gets or sets a value indicating whether [settings was persisted] to disk.</summary>
        /// <value>
        ///   <c>true</c> if [settings persisted]; otherwise, <c>false</c>.</value>
        public bool SettingsPersisted
        {
            get; set;
        }
    }
}
