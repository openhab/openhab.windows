using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace OpenHAB.Core.Model
{
    /// <summary>Used to serialize the service information from openHAB server.</summary>
    public partial class RuntimeInfo
    {
        /// <summary>Gets or sets the server version.</summary>
        /// <value>The version.</value>
        [JsonProperty("version")]
        public string Version
        {
            get; set;
        }

        /// <summary>Gets or sets the build version string.</summary>
        /// <value>The build string.</value>
        [JsonProperty("buildString")]
        public string BuildString
        {
            get; set;
        }
    }
}
