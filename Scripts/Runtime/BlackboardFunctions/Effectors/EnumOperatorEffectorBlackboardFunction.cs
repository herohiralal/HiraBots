using AOT;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class EnumOperatorEffectorBlackboardFunction : EffectorBlackboardFunction
    {
        [System.Serializable]
        private enum OperationType
        {
            Set,
            AddFlags,
            RemoveFlags,
        }

        private struct Memory
        {
            internal ushort m_Offset;
            internal OperationType m_OperationType;
            internal byte m_Value;
        }

        private static bool s_FunctionCompiled = false;
        private static FunctionPointer<EffectorDelegate> s_Function;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            if (!s_FunctionCompiled)
            {
                s_Function = BurstCompiler.CompileFunctionPointer<EffectorDelegate>(ActualFunction);
                s_FunctionCompiled = true;
            }
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        [Tooltip("The type of operation to perform.")]
        [SerializeField] private OperationType m_OperationType = OperationType.Set;

        [Tooltip("The second value to use for the operator.")]
        [SerializeField] private DynamicEnum m_Value = default;

        // memory size override
        protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<Memory>(); // pack memory

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

        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(EffectorDelegate))]
        private static void ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;

            var offset = memory->m_Offset;
            var value = memory->m_Value;

            switch (memory->m_OperationType)
            {
                case OperationType.Set:
                    blackboard.Access<byte>(offset) = value;
                    break;
                case OperationType.AddFlags:
                    blackboard.Access<byte>(offset) |= value;
                    break;
                case OperationType.RemoveFlags:
                    blackboard.Access<byte>(offset) &= value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        // pack memory
        private Memory memory => new Memory
        {
            m_Offset = m_Key.selectedKey.compiledData.memoryOffset,
            m_OperationType = m_OperationType,
            m_Value = m_Value
        };
    }
}