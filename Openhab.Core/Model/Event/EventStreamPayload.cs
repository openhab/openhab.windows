namespace OpenHAB.Core.Model.Event
{
    /// <summary>
    /// The payload coming from the event service.
    /// </summary>
    public class EventStreamPayload
    {
        /// <summary>
        /// Gets or sets the type of event.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the event.
        /// </summary>
        public string Value { get; set; }

        /// <summary>Gets or sets the old type of the event.</summary>
        /// <value>The old type.</value>
        public string OldType
        {
            get;
            set;
        }

        /// <summary>Gets or sets the old value of the event.</summary>
        /// <value>The old value.</value>
        public string OldValue
        {
            get;
            set;
        }
    }
}