using AOT;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class IsSetEffectorBlackboardFunction : EffectorBlackboardFunction
    {
        [System.Serializable]
        private enum OperationType
        {
            Set,
            Unset,
        }

        private struct Memory
        {
            internal BlackboardKeyType m_KeyType;
            internal ushort m_Offset;
            internal OperationType m_OperationType;
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

            switch (memory->m_OperationType)
            {
                case OperationType.Set:
                    switch (memory->m_KeyType)
                    {
                        case BlackboardKeyType.Boolean:
                            blackboard.Access<bool>(offset) = true;
                            break;
                        case BlackboardKeyType.Quaternion:
                            blackboard.Access<quaternion>(offset) = quaternion.Euler(new float3(10));
                            break;
                        case BlackboardKeyType.Vector:
                            blackboard.Access<float3>(offset) = 1;
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException($"Invalid key type: {memory->m_KeyType}");
                    }

                    break;
                case OperationType.Unset:
                    switch (memory->m_KeyType)
                    {
                        case BlackboardKeyType.Boolean:
                            blackboard.Access<bool>(offset) = false;
                            break;
                        case BlackboardKeyType.Quaternion:
                            blackboard.Access<quaternion>(offset) = quaternion.identity;
                            break;
                        case BlackboardKeyType.Vector:
                            blackboard.Access<float3>(offset) = float3.zero;
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException($"Invalid key type: {memory->m_KeyType}");
                    }

                    break;
                default:
                    throw new System.ArgumentOutOfRangeException($"Unknown operation type: {memory->m_OperationType}");
            }
        }

        // pack memory
        private Memory memory => new Memory
        {
            m_KeyType = m_Key.selectedKey.keyType,
            m_Offset = m_Key.selectedKey.compiledData.memoryOffset,
            m_OperationType = m_OperationType
        };
    }
}