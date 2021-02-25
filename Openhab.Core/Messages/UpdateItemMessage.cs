namespace OpenHAB.Core.Messages
{
    /// <summary>
    /// A message to trigger an item update coming from a server event.
    /// </summary>
    public class UpdateItemMessage
    {
        /// <summary>Initializes a new instance of the <see cref="UpdateItemMessage" /> class.</summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="value">The value.</param>
        public UpdateItemMessage(string itemName, string value)
        {
            ItemName = itemName;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the name of the updated item.
        /// </summary>
        public string ItemName
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the updated value.
        /// </summary>
        public string Value
        {
            get; set;
        }
    }
}