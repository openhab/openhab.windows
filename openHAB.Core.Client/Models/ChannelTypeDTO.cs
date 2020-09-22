// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace openHAB.Core.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ChannelTypeDTO
    {
        /// <summary>
        /// Initializes a new instance of the ChannelTypeDTO class.
        /// </summary>
        public ChannelTypeDTO()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ChannelTypeDTO class.
        /// </summary>
        public ChannelTypeDTO(IList<ConfigDescriptionParameterDTO> parameters = default(IList<ConfigDescriptionParameterDTO>), IList<ConfigDescriptionParameterGroupDTO> parameterGroups = default(IList<ConfigDescriptionParameterGroupDTO>), string description = default(string), string label = default(string), string category = default(string), string itemType = default(string), string kind = default(string), StateDescription stateDescription = default(StateDescription), IList<string> tags = default(IList<string>), string uID = default(string), bool? advanced = default(bool?), CommandDescription commandDescription = default(CommandDescription))
        {
            Parameters = parameters;
            ParameterGroups = parameterGroups;
            Description = description;
            Label = label;
            Category = category;
            ItemType = itemType;
            Kind = kind;
            StateDescription = stateDescription;
            Tags = tags;
            UID = uID;
            Advanced = advanced;
            CommandDescription = commandDescription;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "parameters")]
        public IList<ConfigDescriptionParameterDTO> Parameters { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "parameterGroups")]
        public IList<ConfigDescriptionParameterGroupDTO> ParameterGroups { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "itemType")]
        public string ItemType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "stateDescription")]
        public StateDescription StateDescription { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        public IList<string> Tags { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "UID")]
        public string UID { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "advanced")]
        public bool? Advanced { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "commandDescription")]
        public CommandDescription CommandDescription { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Tags != null)
            {
                if (Tags.Count != System.Linq.Enumerable.Count(System.Linq.Enumerable.Distinct(Tags)))
                {
                    throw new ValidationException(ValidationRules.UniqueItems, "Tags");
                }
            }
        }
    }
}
