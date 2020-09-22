// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace openHAB.Core.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ConfigDescriptionDTO
    {
        /// <summary>
        /// Initializes a new instance of the ConfigDescriptionDTO class.
        /// </summary>
        public ConfigDescriptionDTO()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ConfigDescriptionDTO class.
        /// </summary>
        public ConfigDescriptionDTO(string uri = default(string), IList<ConfigDescriptionParameterDTO> parameters = default(IList<ConfigDescriptionParameterDTO>), IList<ConfigDescriptionParameterGroupDTO> parameterGroups = default(IList<ConfigDescriptionParameterGroupDTO>))
        {
            Uri = uri;
            Parameters = parameters;
            ParameterGroups = parameterGroups;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "parameters")]
        public IList<ConfigDescriptionParameterDTO> Parameters { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "parameterGroups")]
        public IList<ConfigDescriptionParameterGroupDTO> ParameterGroups { get; set; }

    }
}
