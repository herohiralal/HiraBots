using UnityEngine;

namespace HiraBots
{
    internal unsafe class BooleanBlackboardKey : BlackboardKey
    {
        internal BooleanBlackboardKey() : base(sizeof(byte), BlackboardKeyType.Boolean)
        {
        }

        [SerializeField] private bool defaultValue = false;

        protected override void CompileInternal(IBlackboardKeyCompilerContext context) =>
            BlackboardUnsafeHelpers.WriteBooleanValue(context.Address, 0, defaultValue);
    }
}