using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Quaternion blackboard key.
    /// </summary>
    internal partial class QuaternionBlackboardKey : BlackboardKey
    {
        internal unsafe QuaternionBlackboardKey()
        {
            m_SizeInBytesInternal = (byte) sizeof(Quaternion);
            m_KeyType = BlackboardKeyType.Quaternion;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private Vector3 m_DefaultValue = Vector3.zero;
    }
}