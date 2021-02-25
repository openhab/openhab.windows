using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenHAB.Core.Model.Event;

namespace OpenHAB.Core.Services
{
    /// <inheritdoc/>
    public class OpenHABEventParser : IOpenHABEventParser
    {
        private ILogger<OpenHABEventParser> _logger;

        /// <summary>Initializes a new instance of the <see cref="OpenHABEventParser" /> class.</summary>
        /// <param name="logger">The logger.</param>
        public OpenHABEventParser(ILogger<OpenHABEventParser> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public OpenHABEvent Parse(string message)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<EventStreamData>(message.Remove(0, 6));

                if (data.Type == "ThingUpdatedEvent" || data.Type == "ThingStatusInfoChangedEvent")
                {
                    _logger.LogWarning($"Event type is not supported '{data.Type}'.");
                    return null;
                }

                var payload = JsonConvert.DeserializeObject<EventStreamPayload>(data.Payload);
                string itemName = data.Topic
                                        .Replace("openhab/items/", string.Empty) // openHAB V3
                                        .Replace("smarthome/items/", string.Empty) // openHAB V2
                                        .Replace("/statechanged", string.Empty).Replace("/state", string.Empty);

                if (!Enum.TryParse(typeof(OpenHABEventType), data.Type, out object type))
                {
                    type = OpenHABEventType.Unknown;
                }

                OpenHABEvent openHABevent = new OpenHABEvent()
                {
                    ItemName = itemName,
                    ValueType = payload.Type,
                    Value = payload.Value,
                    Topic = data.Topic,
                    OldType = payload.OldType,
                    OldValue = payload.OldValue,
                    EventType = (OpenHABEventType)type,
                };

                return openHABevent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse openHAB event.");
            }

            return null;
        }
    }
}
