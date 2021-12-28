using AOT;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Perform a unary operation on an enum.
    /// </summary>
    [BurstCompile]
    internal unsafe partial class EnumOperatorEffectorBlackboardFunction : EffectorBlackboardFunction
    {
        [System.Serializable]
        internal enum OperationType
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
                    blackboard.Access<byte>(offset) = (byte) (blackboard.Access<byte>(offset) & ~value);
                    break;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                default:
                    throw new System.ArgumentOutOfRangeException();
#endif
            }
        }

        // non-VM execution
        protected override void ExecuteFunction(BlackboardComponent blackboard, bool expected)
        {
            var currentValue = blackboard.GetEnumValue(m_Key.selectedKey.name);

            switch (m_OperationType)
            {
                case OperationType.Set:
                    blackboard.SetEnumValue(m_Key.selectedKey.name, m_Value, expected);
                    break;
                case OperationType.AddFlags:
                    blackboard.SetEnumValue(m_Key.selectedKey.name, (byte) (currentValue | m_Value), expected);
                    break;
                case OperationType.RemoveFlags:
                    blackboard.SetEnumValue(m_Key.selectedKey.name, (byte) (currentValue & ~(byte)(m_Value)), expected);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}