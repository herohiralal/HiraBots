using AOT;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Perform a set/unset operation on a boolean, a quaternion, or a vector.
    /// </summary>
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

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        [Tooltip("The type of operation to perform.")]
        [SerializeField] private OperationType m_OperationType = OperationType.Set;

        // pack memory
        private Memory memory => new Memory
        {
            m_KeyType = m_Key.selectedKey.keyType,
            m_Offset = m_Key.selectedKey.compiledData.memoryOffset,
            m_OperationType = m_OperationType
        };

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
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                        default:
                            throw new System.ArgumentOutOfRangeException($"Invalid key type: {memory->m_KeyType}");
#endif
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
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                        default:
                            throw new System.ArgumentOutOfRangeException($"Invalid key type: {memory->m_KeyType}");
#endif
                    }

                    break;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                default:
                    throw new System.ArgumentOutOfRangeException($"Unknown operation type: {memory->m_OperationType}");
#endif
            }
        }
    }
}