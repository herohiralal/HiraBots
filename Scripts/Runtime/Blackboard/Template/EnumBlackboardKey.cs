using UnityEngine;

namespace HiraBots
{
    internal partial class EnumBlackboardKey : BlackboardKey
    {
        internal EnumBlackboardKey()
        {
            SizeInBytesInternal = sizeof(byte);
            KeyType = BlackboardKeyType.Enum;
        }

        [SerializeField] private DynamicEnum defaultValue = default;
    }
}