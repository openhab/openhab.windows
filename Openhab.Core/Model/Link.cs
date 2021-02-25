using System;
using Newtonsoft.Json;

namespace OpenHAB.Core.Model
{
    /// <summary>Use to serialize api information from openHAB server.</summary>
    public partial class Link
    {
        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        [JsonProperty("type")]
        public string Type
        {
            get; set;
        }

        /// <summary>Gets or sets the URL.</summary>
        /// <value>The URL.</value>
        [JsonProperty("url")]
        public Uri Url
        {
            get; set;
        }
    }
}
