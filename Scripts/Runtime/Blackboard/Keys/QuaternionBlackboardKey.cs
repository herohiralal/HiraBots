using Unity.Mathematics;
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
            m_SizeInBytesInternal = (byte) sizeof(quaternion);
            m_KeyType = BlackboardKeyType.Quaternion;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private float3 m_DefaultValue = float3.zero;
    }
}