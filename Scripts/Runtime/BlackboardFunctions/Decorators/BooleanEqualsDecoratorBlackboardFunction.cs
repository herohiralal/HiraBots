using AOT;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class BooleanEqualsDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        private struct Memory
        {
            public ushort m_Offset;
            public bool m_Value;
        }

        private static readonly FunctionPointer<DecoratorDelegate> s_Function;

        static BooleanEqualsDecoratorBlackboardFunction()
        {
            s_Function = BurstCompiler.CompileFunctionPointer<DecoratorDelegate>(ActualFunction);
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardKey m_Key = null;
        // todo: create a blackboard key picking system which can filter based on type and a select blackboard

        [Tooltip("The value to compare.")]
        [SerializeField] private bool m_Value = true;

        // memory size override
        protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<Memory>(); // pack memory

        // compile override
        protected internal override byte* Compile(byte* stream)
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
        private static bool ActualFunction(in LowLevelBlackboard blackboard, byte* memory)
        {
            var actualMemory = (Memory*) memory;
            return blackboard.Access<bool>(actualMemory->m_Offset) == actualMemory->m_Value;
        }

        // pack memory
        private Memory memory => new Memory {m_Offset = m_Key.compiledData.memoryOffset, m_Value = m_Value};
    }
}