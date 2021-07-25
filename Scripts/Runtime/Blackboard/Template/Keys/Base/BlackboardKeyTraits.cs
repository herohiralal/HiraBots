using System;

namespace HiraBots
{
    [Serializable, Flags]
    internal enum BlackboardKeyTraits : byte
    {
        None = 0,
        InstanceSynced = 1 << 0,
        BroadcastEventOnUnexpectedChange = 1 << 1
    }
}