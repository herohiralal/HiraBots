using System;
using UnityEngine;

namespace HiraBots
{
    internal abstract class BlackboardKey : ScriptableObject
    {
        protected internal BlackboardKey(int sizeInBytes, BlackboardKeyType keyType) =>
            (SizeInBytes, KeyType) = (sizeInBytes, keyType);

        [SerializeField, HideInInspector] private bool instanceSynced = false;
        [SerializeField, HideInInspector] private bool essentialToDecisionMaking = false;

        [NonSerialized] protected internal BlackboardKeyCompiledData CompiledDataInternal = null;
        [NonSerialized] internal readonly int SizeInBytes;
        [NonSerialized] internal readonly BlackboardKeyType KeyType;

        internal BlackboardKeyCompiledData CompiledData => CompiledDataInternal;

        public bool IsCompiled => CompiledData != null;

        internal BlackboardKeyCompiledData Compile(IBlackboardKeyCompilerContext context)
        {
            CompileInternal(context);
            var traits = BlackboardKeyTraits.None
                         | (instanceSynced ? BlackboardKeyTraits.InstanceSynced : BlackboardKeyTraits.None)
                         | (essentialToDecisionMaking ? BlackboardKeyTraits.BroadcastEventOnUnexpectedChange : BlackboardKeyTraits.None);
            return CompiledDataInternal = new BlackboardKeyCompiledData(context.Identifier, context.Index, traits, KeyType);
        }

        protected abstract void CompileInternal(IBlackboardKeyCompilerContext context);

        internal virtual void Free() => CompiledDataInternal = null;

#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode)

#pragma warning disable CS0414
        [SerializeField, HideInInspector] private bool isExpanded = false;
#pragma warning restore CS0414

        internal void Validate(IBlackboardKeyValidatorContext context)
        {
            if (SizeInBytes < byte.MinValue || SizeInBytes > byte.MaxValue || KeyType == BlackboardKeyType.Invalid)
                context.MarkUnsuccessful();
        }

#endif
    }
}