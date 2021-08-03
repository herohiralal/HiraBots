using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// 8-bit unsigned Enum blackboard key.
    /// </summary>
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