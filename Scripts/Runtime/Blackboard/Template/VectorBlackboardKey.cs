using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Vector blackboard key.
    /// </summary>
    internal partial class VectorBlackboardKey : BlackboardKey
    {
        internal unsafe VectorBlackboardKey()
        {
            m_SizeInBytesInternal = (byte) sizeof(float3);
            m_KeyType = BlackboardKeyType.Vector;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private float3 m_DefaultValue = float3.zero;
    }
}