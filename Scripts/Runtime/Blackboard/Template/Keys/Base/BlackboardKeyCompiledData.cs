namespace HiraBots
{
    internal class BlackboardKeyCompiledData
    {
        internal BlackboardKeyCompiledData(ushort memoryOffset, ushort index, BlackboardKeyTraits traits, BlackboardKeyType keyType)
        {
            MemoryOffset = memoryOffset;
            Index = index;
            Traits = traits;
            KeyType = keyType;
        }

        internal readonly ushort MemoryOffset;
        internal readonly ushort Index;
        internal readonly BlackboardKeyTraits Traits;
        internal readonly BlackboardKeyType KeyType;
    }
}