using OpenHAB.Core.Model;

namespace OpenHAB.Core.Services
{
    /// <summary>Class handles toast and tile notifications.</summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Resets the notification count on badge.
        /// </summary>
        void ResetBadgeCount();
    }
}