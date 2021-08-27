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

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        [Tooltip("The type of operation to perform.")]
        [SerializeField] private OperationType m_OperationType = OperationType.Set;

        [Tooltip("The second value to use for the operator.")]
        [SerializeField] private DynamicEnum m_Value = default;

        // pack memory
        private Memory memory => new Memory
        {
            m_Offset = m_Key.selectedKey.compiledData.memoryOffset,
            m_OperationType = m_OperationType,
            m_Value = m_Value
        };

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
    }
}