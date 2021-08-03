namespace HiraBots
{
    /// <summary>
    /// The type of key a blackboard can have.
    /// Acts as quick RTTI to maintain type-safety.
    /// </summary>
    internal enum BlackboardKeyType : byte
    {
        Invalid = 0,
        Boolean,
        Enum,
        Float,
        Integer,
        Object,
        Quaternion,
        Vector
    }
}