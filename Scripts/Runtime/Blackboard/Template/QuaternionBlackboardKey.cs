using UnityEngine;

namespace HiraBots
{
    internal partial class QuaternionBlackboardKey : BlackboardKey
    {
        internal QuaternionBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(float) * 4;
            m_KeyType = BlackboardKeyType.Quaternion;
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;
    }
}