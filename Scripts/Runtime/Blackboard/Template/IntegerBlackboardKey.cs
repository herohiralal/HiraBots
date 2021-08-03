using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Integer blackboard key.
    /// </summary>
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