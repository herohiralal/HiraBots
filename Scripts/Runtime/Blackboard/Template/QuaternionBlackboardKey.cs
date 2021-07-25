using UnityEngine;

namespace HiraBots
{
    internal partial class QuaternionBlackboardKey : BlackboardKey
    {
        internal QuaternionBlackboardKey() : base(sizeof(float) * 4, BlackboardKeyType.Quaternion)
        {
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;
    }
}