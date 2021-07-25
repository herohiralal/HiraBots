using UnityEngine;

namespace HiraBots
{
    internal unsafe class ObjectBlackboardKey : BlackboardKey
    {
        internal ObjectBlackboardKey() : base(sizeof(int), BlackboardKeyType.Object)
        {
        }

        [SerializeField] private Object defaultValue = null;

        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteObjectValue(context.Address, 0, defaultValue);
    }
}