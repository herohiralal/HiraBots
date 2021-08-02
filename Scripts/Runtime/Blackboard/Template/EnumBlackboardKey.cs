using UnityEngine;

namespace HiraBots
{
    internal partial class EnumBlackboardKey : BlackboardKey
    {
        internal EnumBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(byte);
            m_KeyType = BlackboardKeyType.Enum;
        }

        [SerializeField] private DynamicEnum m_DefaultValue = default;
    }
}