using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Quaternion blackboard key.
    /// </summary>
    internal partial class QuaternionBlackboardKey : BlackboardKey
    {
        internal QuaternionBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(float) * 4;
            m_KeyType = BlackboardKeyType.Quaternion;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private Vector3 m_DefaultValue = Vector3.zero;
    }
}