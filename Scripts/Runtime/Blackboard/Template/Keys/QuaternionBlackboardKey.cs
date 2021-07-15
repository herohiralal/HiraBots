using UnityEngine;

namespace HiraBots
{
    internal unsafe class QuaternionBlackboardKey : BlackboardKey
    {
        public QuaternionBlackboardKey() : base(sizeof(float) * 4, BlackboardKeyType.Quaternion)
        {
        }

        [SerializeField] private Quaternion defaultValue = Quaternion.identity;

        protected override void CompileInternal(IBlackboardKeyCompilerContext context)
        {
            BlackboardUnsafeHelpers.WriteQuaternionValue(context.Address, 0, defaultValue);
        }
    }
}