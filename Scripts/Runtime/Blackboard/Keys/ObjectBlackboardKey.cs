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
            m_SizeInBytes = sizeof(int);
            m_KeyType = BlackboardKeyType.Object;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private Object m_DefaultValue = null;
    }
}