namespace OpenHAB.Core.Model.Event
{
    /// <summary>OpenHAB event.</summary>
    public class OpenHABEvent
    {
        /// <summary>Gets the type of the event.</summary>
        /// <value>The type of the event.</value>
        public OpenHABEventType EventType
        {
            get;
            internal set;
        }

        /// <summary>Gets or sets the name of the item.</summary>
        /// <value>The name of the item.</value>
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the old type.</summary>
        /// <value>The old type.</value>
        public string OldType
        {
            get;
            set;
        }

        /// <summary>Gets or sets the old value.</summary>
        /// <value>The old value.</value>
        public string OldValue
        {
            get;
            set;
        }

        /// <summary>Gets or sets the topic.</summary>
        /// <value>The topic.</value>
        public string Topic
        {
            get;
            set;
        }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        public string Value
        {
            get;
            set;
        }

        /// <summary>Gets or sets the type of the value.</summary>
        /// <value>The type of the value.</value>
        public string ValueType
        {
            get;
            set;
        }
    }
}