using System.Threading.Tasks;

namespace OpenHAB.Core.Services
{
    /// <summary>Manages app behaviour like autostart.</summary>
    public interface IAppManager
    {
        /// <summary>Determines if the autostart can be enabled on system logon.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<bool> CanEnableAutostart();

        /// <summary>Determines whether autostartup on system logon is enabled.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<bool> IsStartupEnabled();

        /// <summary>Toggles the autostart.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        Task ToggleAutostart();
    }
}