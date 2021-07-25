using UnityEngine;

namespace HiraBots
{
    internal partial class VectorBlackboardKey : BlackboardKey
    {
        internal VectorBlackboardKey() : base(sizeof(float) * 3, BlackboardKeyType.Vector)
        {
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;
    }
}