using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Data compiled from a blackboard key.
    /// </summary>
    internal readonly struct BlackboardKeyCompiledData
    {
        internal BlackboardKeyCompiledData(ushort memoryOffset, ushort index, BlackboardKeyTraits traits, BlackboardKeyType keyType)
        {
            this.memoryOffset = memoryOffset;
            this.index = index;
            this.traits = traits;
            this.keyType = keyType;
        }

        /// <summary>
        /// An empty BlackboardKeyCompiledData object.
        /// </summary>
        internal static BlackboardKeyCompiledData none
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new BlackboardKeyCompiledData(ushort.MaxValue, ushort.MaxValue, BlackboardKeyTraits.None, BlackboardKeyType.Invalid);
        }

        /// <summary>
        /// Check whether the data is valid.
        /// </summary>
        internal bool isValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => keyType != BlackboardKeyType.Invalid;
        }

        /// <summary>
        /// The memory offset of the key within all the keys that a blackboard template has.
        /// Inherited or otherwise.
        /// </summary>
        internal ushort memoryOffset { get; }

        /// <summary>
        /// The index of the key within all the keys that a blackboard template has.
        /// Inherited or otherwise.
        /// Node that keys get arranged in an increasing order of their size.
        /// </summary>
        internal ushort index { get; }

        /// <summary>
        /// The traits of the key.
        /// </summary>
        internal BlackboardKeyTraits traits { get; }

        /// <summary>
        /// The variable type of the key.
        /// </summary>
        internal BlackboardKeyType keyType { get; }

        /// <summary>
        /// Whether the key is supposed to have a synchronized instance.
        /// </summary>
        internal bool instanceSynced
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (traits & BlackboardKeyTraits.InstanceSynced) != 0;
        }

        /// <summary>
        /// Whether the key is supposed to broadcast an event on unexpected change.
        /// </summary>
        internal bool broadcastEventOnUnexpectedChange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (traits & BlackboardKeyTraits.BroadcastEventOnUnexpectedChange) != 0;
        }
    }
}