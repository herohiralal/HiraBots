namespace HiraBots
{
    internal unsafe interface IBlackboardKeyCompilerContext
    {
        byte* Address { get; }
        ushort Index { get; }
        ushort Identifier { get; }
    }
}