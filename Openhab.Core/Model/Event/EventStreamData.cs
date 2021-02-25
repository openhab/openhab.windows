namespace OpenHAB.Core.Model.Event
{
    /// <summary>
    /// Data coming from the eventstream.
    /// </summary>
    public class EventStreamData
    {
        /// <summary>
        /// Gets or sets the topic on which the event was fired.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the payload of the event.
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets the type of event.
        /// </summary>
        public string Type { get; set; }
    }
}