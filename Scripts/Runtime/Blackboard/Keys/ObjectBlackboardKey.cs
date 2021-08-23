namespace HiraBots
{
    /// <summary>
    /// Object blackboard key.
    /// </summary>
    internal partial class ObjectBlackboardKey : BlackboardKey
    {
        internal ObjectBlackboardKey()
        {
            m_SizeInBytes = sizeof(int);
            m_KeyType = BlackboardKeyType.Object;
        }
    }
}