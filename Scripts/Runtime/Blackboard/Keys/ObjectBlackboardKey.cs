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

        // cannot let this class support default value because currently
        // it would be a headache to make virtually any object statically
        // available at runtime initialization for code generation backend
        // cooking all the object value references and then resolving them
        // at runtime might help, but needs more testing
    }
}