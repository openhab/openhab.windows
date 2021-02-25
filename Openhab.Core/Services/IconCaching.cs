using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Helpers;
using OpenHAB.Core.Model;
using Windows.Storage;

namespace OpenHAB.Core.Services
{
    /// <inheritdoc/>
    public class IconCaching : IIconCaching
    {
        private string _iconCacheDirectory = "icons";
        private StorageFolder _cacheFolder;
        private ILogger<IconCaching> _logger;

        /// <summary>Initializes a new instance of the <see cref="IconCaching" /> class.</summary>
        /// <param name="logger">The logger.</param>
        public IconCaching(ILogger<IconCaching> logger)
        {
            _logger = logger;
            _cacheFolder = ApplicationData.Current.LocalCacheFolder;
        }

        /// <inheritdoc/>
        public async Task<string> ResolveIconPath(string iconUrl, string iconFormat)
        {
            try
            {
                Match iconName = Regex.Match(iconUrl, "icon/[0-9a-zA-Z]*");
                if (!iconName.Success)
                {
                    throw new OpenHABException("Can not resolve icon name from url");
                }

                StorageFolder storageFolder = await EnsureIconCacheFolder();

                string iconFileName = $"{iconName.Value.Replace("icon/", string.Empty)}.{iconFormat}";
                string iconFilePath = $"{storageFolder.Path}\\{iconFileName}";

                if (await storageFolder.FileExistsAsync(iconFileName))
                {
                    return iconFilePath;
                }

                await DownloadAndSaveIconToCache(iconUrl, iconFileName, storageFolder);
                return iconFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache icon");
                return iconUrl;
            }
        }

        private async Task DownloadAndSaveIconToCache(string iconUrl, string iconFileName, StorageFolder storageFolder)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(new Uri(iconUrl)).ConfigureAwait(false);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return;
                }

                byte[] iconContent = await httpResponse.Content.ReadAsByteArrayAsync();
                StorageFile file = await storageFolder.CreateFileAsync(iconFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(file, iconContent);
            }
        }

        /// <inheritdoc/>
        public async void ClearIconCache()
        {
            StorageFolder storageFolder = await EnsureIconCacheFolder();
            await storageFolder.DeleteAsync();
        }

        private async Task<StorageFolder> EnsureIconCacheFolder()
        {
            StorageFolder storageFolder = await _cacheFolder.TryGetItemAsync(_iconCacheDirectory) as StorageFolder;

            if (storageFolder == null)
            {
                storageFolder = await _cacheFolder.CreateFolderAsync(_iconCacheDirectory);
            }

            return storageFolder;
        }
    }
}
