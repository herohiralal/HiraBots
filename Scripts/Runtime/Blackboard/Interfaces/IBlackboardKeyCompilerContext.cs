namespace HiraBots
{
    internal unsafe interface IBlackboardKeyCompilerContext
    {
        byte* Address { get; }
        bool IsOwner { get; }
        IObjectCache ObjectCache { get; }
        ushort Index { get; }
        ushort Identifier { get; }
    }
}