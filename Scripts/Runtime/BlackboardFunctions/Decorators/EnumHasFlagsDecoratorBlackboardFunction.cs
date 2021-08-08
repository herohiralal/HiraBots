using AOT;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class EnumHasFlagsDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        private struct Memory
        {
            internal ushort m_Offset;
            internal byte m_Value;
        }

        private static readonly FunctionPointer<DecoratorDelegate> s_Function;

        static EnumHasFlagsDecoratorBlackboardFunction()
        {
            s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardKey.Selector m_Key = default;

        [Tooltip("The flags to compare.")]
        [SerializeField] private DynamicEnum m_Value = default;

        // memory size override
        protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<Memory>(); // pack memory

        // compile override
        internal override byte* Compile(byte* stream)
        {
            stream = base.Compile(stream);

            // no offset
            ByteStreamHelpers.Write(ref stream, memory);

            // offset sizeof(Memory)
            return stream;
        }

        // function override
        protected override FunctionPointer<DecoratorDelegate> function => s_Function;

        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(DecoratorDelegate))]
        private static bool ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            return (blackboard.Access<byte>(memory->m_Offset) & memory->m_Value) == memory->m_Value;
        }

        // pack memory
        private Memory memory => new Memory {m_Offset = m_Key.selectedKey.compiledData.memoryOffset, m_Value = m_Value.m_Value};
    }
}