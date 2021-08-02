using UnityEngine;

namespace HiraBots
{
    internal partial class FloatBlackboardKey : BlackboardKey
    {
        internal FloatBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(float);
            m_KeyType = BlackboardKeyType.Float;
        }

        [SerializeField] private float defaultValue = 0f;
    }
}