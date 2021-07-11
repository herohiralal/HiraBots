using System;

namespace HiraBots
{
    [Serializable, Flags]
    internal enum BlackboardKeyTraits : byte
    {
        None = 0,
        InstanceSynced = 1 << 0,
        BroadcastEventOnUnexpectedChange = 1 << 1,
        BooleanKey = 1 << 2,
        EnumKey = 1 << 3, // todo: make an actual enum key, with a callback that writes a ScriptableObject upon compilation
        FloatKey = 1 << 4,
        IntegerKey = 1 << 5,
        ObjectKey = 1 << 6,
        VectorKey = 1 << 7
    }
}