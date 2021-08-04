using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Vector3 blackboard key.
    /// </summary>
    internal partial class VectorBlackboardKey : BlackboardKey
    {
        internal unsafe VectorBlackboardKey()
        {
            m_SizeInBytesInternal = (byte) sizeof(Vector3);
            m_KeyType = BlackboardKeyType.Vector;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private Vector3 m_DefaultValue = Vector3.zero;
    }
}