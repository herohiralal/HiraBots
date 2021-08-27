using Unity.Burst;

namespace HiraBots
{
    internal unsafe partial class EnumOperatorEffectorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<EffectorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<EffectorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        // compile override
        public override void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            // no offset
            ByteStreamHelpers.Write(ref stream, memory);

            // offset sizeof(Memory)
        }

        // function override
        protected override FunctionPointer<EffectorDelegate> function => s_Function;
    }

    internal unsafe partial class FloatOperatorEffectorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<EffectorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<EffectorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        // compile override
        public override void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            // no offset
            ByteStreamHelpers.Write(ref stream, memory);

            // offset sizeof(Memory)
        }

        // function override
        protected override FunctionPointer<EffectorDelegate> function => s_Function;
    }

    internal unsafe partial class IntegerOperatorEffectorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<EffectorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<EffectorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        // compile override
        public override void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            // no offset
            ByteStreamHelpers.Write(ref stream, memory);

            // offset sizeof(Memory)
        }

        // function override
        protected override FunctionPointer<EffectorDelegate> function => s_Function;
    }

    internal unsafe partial class IsSetEffectorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<EffectorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<EffectorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        // compile override
        public override void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            // no offset
            ByteStreamHelpers.Write(ref stream, memory);

            // offset sizeof(Memory)
        }

        // function override
        protected override FunctionPointer<EffectorDelegate> function => s_Function;
    }

    internal unsafe partial class ObjectEqualsEffectorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<EffectorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<EffectorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        // compile override
        public override void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            // no offset
            ByteStreamHelpers.Write(ref stream, memory);

            // offset sizeof(Memory)
        }

        // function override
        protected override FunctionPointer<EffectorDelegate> function => s_Function;
    }
}