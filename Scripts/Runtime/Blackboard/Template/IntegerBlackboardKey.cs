using UnityEngine;

namespace HiraBots
{
    internal partial class IntegerBlackboardKey : BlackboardKey
    {
        internal IntegerBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(int);
            m_KeyType = BlackboardKeyType.Integer;
        }

        [SerializeField] private int m_DefaultValue = 0;
    }
}