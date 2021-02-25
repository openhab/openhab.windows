namespace OpenHAB.Core.Model.Event
{
    /// <summary>openHAB event types.</summary>
    public enum OpenHABEventType
    {
        /// <summary>Item state event.</summary>
        ItemStateEvent = 0,

        /// <summary>Thing updated event.</summary>
        ThingUpdatedEvent = 1,

        /// <summary>Rule status information event.</summary>
        RuleStatusInfoEvent = 2,

        /// <summary>Item state predicted event.</summary>
        ItemStatePredictedEvent = 3,

        /// <summary>Group item state changed event.</summary>
        GroupItemStateChangedEvent = 4,

        /// <summary>Item state changed event.</summary>
        ItemStateChangedEvent = 5,

        /// <summary>Unknown event.</summary>
        Unknown = 6
    }
}