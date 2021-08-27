using AOT;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Perform assignment operation on a certain object.
    /// </summary>
    [BurstCompile]
    internal unsafe partial class ObjectEqualsEffectorBlackboardFunction : EffectorBlackboardFunction
    {
        private struct Memory
        {
            internal ushort m_Offset;
            internal int m_InstanceID;
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        [Tooltip("The value to set.")]
        [SerializeField] private Object m_Value = null;

        // pack memory
        private Memory memory => new Memory
        {
            m_Offset = m_Key.selectedKey.compiledData.memoryOffset,
            m_InstanceID = m_Value == null ? 0 : m_Value.GetInstanceID()
        };

        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(EffectorDelegate))]
        private static void ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            blackboard.Access<int>(memory->m_Offset) = memory->m_InstanceID;
        }
    }
}