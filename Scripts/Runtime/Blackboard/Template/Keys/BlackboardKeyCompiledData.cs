namespace HiraBots
{
    internal class BlackboardKeyCompiledData
    {
        internal BlackboardKeyCompiledData(ushort identifier, ushort index, BlackboardKeyTraits traits, BlackboardKeyType keyType)
        {
            Identifier = identifier;
            Index = index;
            Traits = traits;
            KeyType = keyType;
        }

        internal readonly ushort Identifier;
        internal readonly ushort Index;
        internal readonly BlackboardKeyTraits Traits;
        internal readonly BlackboardKeyType KeyType;
    }
}