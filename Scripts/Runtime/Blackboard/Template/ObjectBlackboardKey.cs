using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Object blackboard key.
    /// </summary>
    internal partial class ObjectBlackboardKey : BlackboardKey
    {
        internal ObjectBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(int);
            m_KeyType = BlackboardKeyType.Object;
        }

        [SerializeField] private Object m_DefaultValue = null;
    }
}