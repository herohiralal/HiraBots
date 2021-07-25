namespace HiraBots
{
    internal unsafe interface IBlackboardKeyCompilerContext
    {
        byte* Address { get; }
        ushort Index { get; }
        ushort MemoryOffset { get; }
        BlackboardKeyCompiledData CompiledData { set; }
        string Name { set; }
    }
}