using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Boolean blackboard key.
    /// </summary>
    internal partial class BooleanBlackboardKey : BlackboardKey
    {
        internal BooleanBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(byte);
            m_KeyType = BlackboardKeyType.Boolean;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private bool m_DefaultValue = false;
    }
}