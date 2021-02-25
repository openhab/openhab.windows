using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Response for REST Root call, which provides all information about openHAB instance.
    /// </summary>
    public class OpenHABAPIInfo
    {
        /// <summary>Gets or sets the version.</summary>
        /// <value>The version.</value>
        [JsonProperty("version")]
        public int Version
        {
            get; set;
        }

        /// <summary>Gets or sets the locale.</summary>
        /// <value>The locale.</value>
        [JsonProperty("locale")]
        public string Locale
        {
            get; set;
        }

        /// <summary>Gets or sets the measurement system.</summary>
        /// <value>The measurement system.</value>
        [JsonProperty("measurementSystem")]
        public string MeasurementSystem
        {
            get; set;
        }

        /// <summary>Gets or sets the runtime information.</summary>
        /// <value>The runtime information.</value>
        [JsonProperty("runtimeInfo")]
        public RuntimeInfo RuntimeInfo
        {
            get; set;
        }

        /// <summary>Gets or sets the links.</summary>
        /// <value>The links.</value>
        [JsonProperty("links")]
        public List<Link> Links
        {
            get; set;
        }
    }
}
