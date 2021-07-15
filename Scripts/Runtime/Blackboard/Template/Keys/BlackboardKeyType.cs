namespace HiraBots
{
    internal enum BlackboardKeyType : byte
    {
        Invalid = 0,
        Boolean,
        Enum, // todo: make an actual enum key, with a callback that writes a ScriptableObject upon compilation
        Float,
        Integer,
        Object,
        Quaternion,
        Vector
    }
}