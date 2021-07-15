using System;
using UnityEngine;

namespace HiraBots
{
    internal abstract class BlackboardKey : ScriptableObject
    {
        protected internal BlackboardKey(int sizeInBytes, BlackboardKeyType keyType) =>
            (SizeInBytes, KeyType) = (sizeInBytes, keyType);

        [SerializeField, HideInInspector] private BlackboardKeyTraits traits = BlackboardKeyTraits.None;

        [NonSerialized] protected internal BlackboardKeyCompiledData CompiledDataInternal = null;
        [NonSerialized] internal readonly int SizeInBytes;
        [NonSerialized] internal readonly BlackboardKeyType KeyType;

        internal BlackboardKeyCompiledData CompiledData => CompiledDataInternal;

        public bool IsCompiled => CompiledData != null;

        internal BlackboardKeyCompiledData Compile(IBlackboardKeyCompilerContext context)
        {
            CompileInternal(context);
            return CompiledDataInternal = new BlackboardKeyCompiledData(context.Identifier, context.Index, traits, KeyType);
        }

        protected abstract void CompileInternal(IBlackboardKeyCompilerContext context);

        internal virtual void Free() => CompiledDataInternal = null;

#if UNITY_EDITOR // ideally validation is only needed within the editor (either when building, or when exiting play mode)

        internal void Validate(IBlackboardKeyValidatorContext context)
        {
            if (SizeInBytes < byte.MinValue || SizeInBytes > byte.MaxValue || KeyType == BlackboardKeyType.Invalid)
                context.MarkUnsuccessful();
        }

#endif
    }
}