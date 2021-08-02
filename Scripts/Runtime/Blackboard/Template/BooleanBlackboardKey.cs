using UnityEngine;

namespace HiraBots
{
    internal partial class BooleanBlackboardKey : BlackboardKey
    {
        internal BooleanBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(byte);
            m_KeyType = BlackboardKeyType.Boolean;
        }

        [SerializeField] private bool m_DefaultValue = false;
    }
}