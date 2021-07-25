using UnityEngine;

namespace HiraBots
{
    internal unsafe partial class QuaternionBlackboardKey : BlackboardKey
    {
        internal QuaternionBlackboardKey() : base(sizeof(float) * 4, BlackboardKeyType.Quaternion)
        {
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;

        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteQuaternionValue(context.Address, 0, Quaternion.Euler(defaultValue));
        }
    }
}