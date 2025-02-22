using System;

namespace HiraBots
{
    /// <summary>
    /// The traits for a blackboard key.
    /// </summary>
    [Serializable, Flags]
    internal enum BlackboardKeyTraits : byte
    {
        None = 0,
        InstanceSynced = 1 << 0, // whether the value is supposed to be synced between all instances
        BroadcastEventOnUnexpectedChange = 1 << 1 // whether an event should be broadcast upon unexpected change in this value
    }
}