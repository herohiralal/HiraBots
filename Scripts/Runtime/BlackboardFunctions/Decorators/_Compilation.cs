#if UNITY_EDITOR
using Unity.Burst;

namespace HiraBots
{
    internal unsafe partial class AlwaysSucceedDecoratorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<DecoratorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        // function override
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;
    }

    internal unsafe partial class EnumHasFlagsDecoratorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<DecoratorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
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
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;
    }

    internal unsafe partial class IsSetDecoratorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<DecoratorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
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
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;
    }

    internal unsafe partial class NumericalComparisonDecoratorBlackboardFunction
    {
        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<DecoratorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
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
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;
    }

    internal unsafe partial class ObjectEqualsDecoratorBlackboardFunction
    {

        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<DecoratorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += ByteStreamHelpers.CombinedSizes<Memory>();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
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
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;
    }
}
#endif