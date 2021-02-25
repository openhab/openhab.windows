using OpenHAB.Core.Model;

namespace OpenHAB.Core.Services
{
    /// <summary>Stores and Fetches data about openHabItems.</summary>
    public interface IItemManager
    {
        /// <summary>Tries to find openHabItem in dictionary, when not available queries data from openHab server.</summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   Is true if the item was found in the dictionary or was successfully downloaded from openHAB server, otherwise false.
        /// </returns>
        bool TryGetItem(string itemName, out OpenHABItem item);
    }
}