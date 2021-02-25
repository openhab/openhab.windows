using System.Collections.Generic;
using System.Threading.Tasks;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;

namespace OpenHAB.Core.Services
{
    /// <inheritdoc/>
    public class ItemManager : IItemManager
    {
        private Dictionary<string, OpenHABItem> _nameToStateDictionary;
        private IOpenHAB _openHABClient;

        /// <summary>Initializes a new instance of the <see cref="ItemManager" /> class.</summary>
        public ItemManager(IOpenHAB openHABClient)
        {
            _nameToStateDictionary = new Dictionary<string, OpenHABItem>();
            _openHABClient = openHABClient;
        }

        /// <inheritdoc/>
        public bool TryGetItem(string itemName, out OpenHABItem item)
        {
            if (!_nameToStateDictionary.TryGetValue(itemName, out item))
            {
                item = _openHABClient.GetItemByName(itemName).Result;
                return _nameToStateDictionary.TryAdd(itemName, item);
            }

            return _nameToStateDictionary.TryGetValue(itemName, out item);
        }
    }
}
