using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.Model.Connection;
using OpenHAB.Core.Model.Event;
using OpenHAB.Core.Services;

namespace OpenHAB.Core.SDK
{
    /// <summary>
    /// The main SDK implementation of the connection to OpenHAB.
    /// </summary>
    public class OpenHABClient : IOpenHAB
    {
        private readonly IMessenger _messenger;
        private readonly ILogger<OpenHABClient> _logger;
        private readonly ISettingsService _settingsService;
        private OpenHABConnection _connection;
        private OpenHABHttpClient _openHABHttpClient;
        private IOpenHABEventParser _eventParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABClient"/> class.
        /// </summary>
        /// <param name="settingsService">The service to fetch the settings.</param>
        /// <param name="messenger">The messenger instance.</param>
        /// <param name="eventParser">openHAB event parser.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="openHABHttpClient">OpenHab Http client factory.</param>
        public OpenHABClient(
            ISettingsService settingsService,
            IMessenger messenger,
            IOpenHABEventParser eventParser,
            ILogger<OpenHABClient> logger,
            OpenHABHttpClient openHABHttpClient)
        {
            _settingsService = settingsService;
            _messenger = messenger;
            _logger = logger;
            _openHABHttpClient = openHABHttpClient;
            _eventParser = eventParser;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseResult<bool>> CheckUrlReachability(OpenHABConnection connection)
        {
            if (string.IsNullOrWhiteSpace(connection?.Url))
            {
                return new HttpResponseResult<bool>(false, null);
            }

            if (!connection.Url.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
            {
                connection.Url = connection.Url + "/";
            }

            try
            {
                Settings settings = _settingsService.Load();
                var client = _openHABHttpClient.DisposableClient(connection, settings);
                var result = await client.GetAsync(connection.Url + "rest").ConfigureAwait(false);

                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request for command failed, ErrorCode:'{result.StatusCode}'");
                }

                return new HttpResponseResult<bool>(result.IsSuccessStatusCode, result.StatusCode);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "CheckUrlReachability failed");

                return new HttpResponseResult<bool>(false, null, ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "CheckUrlReachability failed");

                return new HttpResponseResult<bool>(false, null, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckUrlReachability failed.");
                return new HttpResponseResult<bool>(false, null, ex);
            }
        }

        /// <inheritdoc />
        public async Task<OpenHABVersion> GetOpenHABVersion()
        {
            try
            {
                var settings = _settingsService.Load();
                var httpClient = _openHABHttpClient.Client(_connection, settings);

                if (httpClient == null)
                {
                    return OpenHABVersion.None;
                }

                HttpResponseMessage result = await httpClient.GetAsync(Constants.API.ServerInformation).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request get OpenHab version failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string responseBody = await result.Content.ReadAsStringAsync();

                OpenHABAPIInfo apiInfo = JsonConvert.DeserializeObject<OpenHABAPIInfo>(responseBody);
                if (apiInfo.Version < 4)
                {
                    return OpenHABVersion.Three;
                }

                if (!Version.TryParse(apiInfo?.RuntimeInfo.Version, out Version serverVersion))
                {
                    string message = "Not able to parse runtime verion from openHAB server";
                    _logger.LogError(message);

                    throw new OpenHABException(message);
                }

                OpenHABVersion openHABVersion = (OpenHABVersion)serverVersion.Major;

                return openHABVersion;
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<OpenHABWidget>> LoadItemsFromSitemap(OpenHABSitemap sitemap, OpenHABVersion version)
        {
            try
            {
                _logger.LogInformation($"Load sitemaps items for sitemap '{sitemap.Name}'");

                var settings = _settingsService.Load();
                var result = await _openHABHttpClient.Client(_connection, settings).GetAsync(sitemap.Link).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request for loading sitemaps items failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                ICollection<OpenHABWidget> items = null;
                if (version == OpenHABVersion.One)
                {
                    // V1 = xml
                    items = ParseWidgets(resultString);
                }
                else
                {
                    // V2 = JSON
                    var jsonObject = JObject.Parse(resultString);
                    items = JsonConvert.DeserializeObject<List<OpenHABWidget>>(jsonObject["homepage"]["widgets"].ToString());
                }

                _logger.LogInformation($"Loaded '{items.Count}' sitemaps items from server");

                return items;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "LoadItemsFromSitemap failed.");
                throw new OpenHABException("Invalid call", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadItemsFromSitemap failed.");
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<OpenHABItem> GetItemByName(string itemName)
        {
            try
            {
                _logger.LogInformation($"Load item by name '{itemName}' from openHAB server");

                var settings = _settingsService.Load();
                Uri resourceUrl = new Uri($"{Constants.API.Items}/{itemName}", UriKind.Relative);

                var result = await _openHABHttpClient.Client(_connection, settings).GetAsync(resourceUrl).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request to fetch item '{itemName}' failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                OpenHABItem item = JsonConvert.DeserializeObject<OpenHABItem>(resultString);

                _logger.LogInformation($"Loaded item '{itemName}' from server");

                return item;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "LoadItemsFromSitemap failed.");
                throw new OpenHABException("Invalid call", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadItemsFromSitemap failed.");
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<OpenHABSitemap>> LoadSiteMaps(OpenHABVersion version, List<Func<OpenHABSitemap, bool>> filters)
        {
            try
            {
                _logger.LogInformation($"Load sitemaps for OpenHab server version '{version.ToString()}'");

                var settings = _settingsService.Load();
                var result = await _openHABHttpClient.Client(_connection, settings).GetAsync(Constants.API.Sitemaps).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request for loading sitemaps failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                var sitemaps = new List<OpenHABSitemap>();

                // V1 = xml
                if (version == OpenHABVersion.One)
                {
                    XDocument xml = XDocument.Parse(resultString);

                    foreach (XElement xElement in xml.Element("sitemaps").Elements())
                    {
                        var sitemap = new OpenHABSitemap(xElement);
                        sitemaps.Add(sitemap);
                    }

                    return sitemaps;
                }

                // V2 = JSON
                sitemaps = JsonConvert.DeserializeObject<List<OpenHABSitemap>>(resultString);

                _logger.LogInformation($"Loaded '{sitemaps.Count}' sitemaps from server");
                return sitemaps.Where(sitemap =>
                {
                    bool isIncluded = true;
                    filters.ForEach(filter => isIncluded &= filter(sitemap));

                    return isIncluded;
                }).ToList();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "LoadSiteMaps failed.");
                throw new OpenHABException("Invalid call", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadSiteMaps failed.");
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ResetConnection()
        {
            var settings = _settingsService.Load();
            bool isValid = await SetValidUrl(settings);

            if (!isValid)
            {
                return false;
            }

            _openHABHttpClient.ResetClient();

            return true;
        }

        /// <inheritdoc />
        public async Task<HttpResponseResult<bool>> SendCommand(OpenHABItem item, string command)
        {
            try
            {
                if (item == null)
                {
                    _logger.LogInformation("Send command is canceled, because item is null (e.g. widget not linked to item)");
                    return new HttpResponseResult<bool>(false, null, null);
                }

                _logger.LogInformation($"Send Command '{command}' for item '{item.Name} of type '{item.Type}'");

                var settings = _settingsService.Load();
                var client = _openHABHttpClient.Client(_connection, settings);
                var content = new StringContent(command);

                var result = await client.PostAsync(item.Link, content);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request for command failed, ErrorCode:'{result.StatusCode}'");
                }

                return new HttpResponseResult<bool>(result.IsSuccessStatusCode, result.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "SendCommand failed.");
                return new HttpResponseResult<bool>(false, null, ex);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "SendCommand failed.");
                return new HttpResponseResult<bool>(false, null, ex);
            }
        }

        /// <inheritdoc />
        public async void StartItemUpdates(System.Threading.CancellationToken token)
        {
            await Task.Run((Func<Task>)(async () =>
            {
                var settings = _settingsService.Load();
                var client = _openHABHttpClient.Client(_connection, settings);
                var requestUri = Constants.API.Events;

                _logger.LogInformation($"Retrive item updates from '{client.BaseAddress.ToString()}'");

                try
                {
                    var stream = await client.GetStreamAsync((string)requestUri).ConfigureAwait(false);

                    using (var reader = new StreamReader((Stream)stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }

                            var updateEvent = reader.ReadLine();
                            if (updateEvent?.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase) == true)
                            {
                                OpenHABEvent ohEvent = _eventParser.Parse(updateEvent);
                                if (ohEvent == null)
                                {
                                    continue;
                                }

                                switch (ohEvent.EventType)
                                {
                                    case OpenHABEventType.ItemStateEvent:
                                        _messenger.Send(new UpdateItemMessage(ohEvent.ItemName, ohEvent.Value));
                                        break;
                                    case OpenHABEventType.ThingUpdatedEvent:
                                        break;
                                    case OpenHABEventType.RuleStatusInfoEvent:
                                        break;
                                    case OpenHABEventType.ItemStatePredictedEvent:
                                        break;
                                    case OpenHABEventType.GroupItemStateChangedEvent:
                                        break;
                                    case OpenHABEventType.ItemStateChangedEvent:
                                        _messenger.Send(new ItemStateChangedMessage(ohEvent.ItemName, ohEvent.Value, ohEvent.OldValue));
                                        break;
                                    case OpenHABEventType.Unknown:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "StartItemUpdates failed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "StartItemUpdates failed.");
                }
            })).ConfigureAwait(false);
        }

        private ICollection<OpenHABWidget> ParseWidgets(string resultString)
        {
            var xml = XDocument.Parse(resultString);

            return
                xml.Element("sitemap")
                    .Element("homepage")
                    .Elements("widget")
                    .Select(xElement => OpenHABWidgetFactory.Parse(xElement))
                    .ToList();
        }

        private async Task<bool> SetValidUrl(Settings settings)
        {
            _logger.LogInformation("Validate Url");

            var isRunningInDemoMode = settings.IsRunningInDemoMode != null && settings.IsRunningInDemoMode.Value;

            _logger.LogInformation($"App is running in demo mode: {isRunningInDemoMode}");

            // no url configured yet
            if (string.IsNullOrWhiteSpace(settings.LocalConnection?.Url) &&
                string.IsNullOrWhiteSpace(settings.RemoteConnection?.Url) &&
                !isRunningInDemoMode)
            {
                return false;
            }

            if (isRunningInDemoMode)
            {
                _connection = new DemoConnectionProfile().CreateConnection();
                OpenHABHttpClient.BaseUrl = Constants.API.DemoModeUrl;
                return true;
            }

            bool meteredConnection = NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection;
            _logger.LogInformation($"Metered Connection Type: {meteredConnection}");

            if (meteredConnection)
            {
                if (string.IsNullOrEmpty(settings.RemoteConnection?.Url))
                {
                    string message = "No remote url configured";
                    _logger.LogWarning(message);

                    return false;
                }

                OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                _connection = settings.RemoteConnection;

                return true;
            }

            HttpResponseResult<bool> result = await CheckUrlReachability(settings.LocalConnection).ConfigureAwait(false);
            _logger.LogInformation($"OpenHab server is reachable: {result.Content}");

            if (!result.Content)
            {
                 Messenger.Default.Send<FireErrorMessage>(new FireErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
                 return false;
            }

            if (result.Content)
            {
                OpenHABHttpClient.BaseUrl = settings.LocalConnection.Url;
                _connection = settings.LocalConnection;

                return true;
            }
            else
            {
                // If remote URL is configured
                if (string.IsNullOrWhiteSpace(settings.RemoteConnection?.Url))
                {
                    Messenger.Default.Send<FireErrorMessage>(new FireErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
                    _logger.LogWarning($"OpenHab server url is not valid");

                    return false;
                }

                result = await CheckUrlReachability(settings.RemoteConnection).ConfigureAwait(false);
                if (!result.Content)
                {
                    Messenger.Default.Send<FireErrorMessage>(new FireErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
                    _logger.LogWarning($"OpenHab server url is not valid");

                    return false;
                }

                OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                _connection = settings.RemoteConnection;

                return true;
            }
        }
    }
}