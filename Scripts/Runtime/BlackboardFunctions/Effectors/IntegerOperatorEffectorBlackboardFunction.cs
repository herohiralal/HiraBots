﻿using AOT;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Perform a unary operation on an integer.
    /// </summary>
    [BurstCompile]
    internal unsafe partial class IntegerOperatorEffectorBlackboardFunction : EffectorBlackboardFunction
    {
        [System.Serializable]
        private enum OperationType
        {
            Set,
            Add,
            Subtract,
            Multiply,
            Divide,
            BitwiseAnd,
            BitwiseOr,
            BitwiseXor,
        }

        private struct Memory
        {
            internal ushort m_Offset;
            internal OperationType m_OperationType;
            internal int m_Value;
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        [Tooltip("The type of operation to perform.")]
        [SerializeField] private OperationType m_OperationType = OperationType.Set;

        [Tooltip("The second value to use for the operator.")]
        [SerializeField] private int m_Value = 0;

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
                    blackboard.Access<int>(offset) = value;
                    break;
                case OperationType.Add:
                    blackboard.Access<int>(offset) += value;
                    break;
                case OperationType.Subtract:
                    blackboard.Access<int>(offset) -= value;
                    break;
                case OperationType.Multiply:
                    blackboard.Access<int>(offset) *= value;
                    break;
                case OperationType.Divide:
                    blackboard.Access<int>(offset) /= value;
                    break;
                case OperationType.BitwiseAnd:
                    blackboard.Access<int>(offset) &= value;
                    break;
                case OperationType.BitwiseOr:
                    blackboard.Access<int>(offset) |= value;
                    break;
                case OperationType.BitwiseXor:
                    blackboard.Access<int>(offset) ^= value;
                    break;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                default:
                    throw new System.ArgumentOutOfRangeException();
#endif
            }
        }
    }
}