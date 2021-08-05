using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Float blackboard key.
    /// </summary>
    internal partial class FloatBlackboardKey : BlackboardKey
    {
        internal FloatBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(float);
            m_KeyType = BlackboardKeyType.Float;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private float m_DefaultValue = 0f;
    }
}