using AOT;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Checks whether an enum has a certain value, or contains certain bitmasks.
    /// </summary>
    [BurstCompile]
    internal unsafe partial class EnumHasFlagsDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        private struct Memory
        {
            internal ushort m_Offset;
            internal byte m_Value;
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        [Tooltip("The flags to compare.")]
        [SerializeField] private DynamicEnum m_Value = default;

        // pack memory
        private Memory memory => new Memory {m_Offset = m_Key.selectedKey.compiledData.memoryOffset, m_Value = m_Value.m_Value};

        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(DecoratorDelegate))]
        private static bool ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            return (blackboard.Access<byte>(memory->m_Offset) & memory->m_Value) == memory->m_Value;
        }

        // non-VM execution
        protected override bool ExecuteFunction(BlackboardComponent blackboard)
        {
            return (blackboard.GetEnumValue(m_Key.selectedKey.name) & m_Value) == m_Value;
        }
    }
}