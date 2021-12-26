namespace UnityEngine
{
    /// <summary>
    /// The type of a blackboard key.
    /// </summary>
    [System.Serializable]
    public enum BlackboardKeyType : byte
    {
        Invalid = 0,
        Boolean = 1 << 0,
        Enum = 1 << 1,
        Float = 1 << 2,
        Integer = 1 << 3,
        Object = 1 << 4,
        Quaternion = 1 << 5,
        Vector = 1 << 6,
        Numeric = Float | Integer,
        UnmanagedSettable = Boolean | Quaternion | Vector,
        Any = Boolean | Enum | Float | Integer | Object | Quaternion | Vector
    }
}