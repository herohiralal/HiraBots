using System;

namespace HiraBots
{
    internal abstract partial class BlackboardKey
    {
        [NonSerialized] protected internal BlackboardKeyCompiledData CompiledDataInternal = null;
        internal BlackboardKeyCompiledData CompiledData => CompiledDataInternal;

        public bool IsCompiled => CompiledData != null;

        internal void Compile(IBlackboardKeyCompilerContext context)
        {
            if (IsCompiled)
                return;
            
            var traits = BlackboardKeyTraits.None
                         | (instanceSynced ? BlackboardKeyTraits.InstanceSynced : BlackboardKeyTraits.None)
                         | (essentialToDecisionMaking ? BlackboardKeyTraits.BroadcastEventOnUnexpectedChange : BlackboardKeyTraits.None);

            CompiledDataInternal = new BlackboardKeyCompiledData(context.MemoryOffset, context.Index, traits, KeyType);
            context.CompiledData = CompiledDataInternal;
            context.Name = name;
            CompileInternal(context);
        }

        internal void Free()
        {
            FreeInternal();
            CompiledDataInternal = null;
        }

        protected abstract void CompileInternal(IBlackboardKeyCompilerContext context);

        protected virtual void FreeInternal()
        {
        }
    }
}