using AOT;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class NumericalComparisonDecoratorBlackboardFunction : DecoratorBlackboardFunction
    {
        [System.Serializable]
        private enum ComparisonType : byte
        {
            AlmostEqualTo,
            GreaterThan,
            GreaterThanEqualTo,
            LesserThan,
            LesserThanEqualTo,
        }

        private struct Memory
        {
            internal ushort m_Offset;
            internal bool m_IntegerKey;
            internal ComparisonType m_ComparisonType;
            internal float m_CompareTo;
            internal float m_Tolerance;
        }

        [Tooltip("The key to look up.")]
        [SerializeField] private BlackboardTemplate.KeySelector m_Key = default;

        [Tooltip("The type of comparison to perform.")]
        [SerializeField] private ComparisonType m_ComparisonType = ComparisonType.AlmostEqualTo;

        [Tooltip("The value to compare.")]
        [SerializeField] private float m_Value = default;

        [Tooltip("The allowed tolerance if the comparison type is equal to.")]
        [SerializeField] private float m_EqualityTolerance = 0.1f;

        // pack memory
        private Memory memory => new Memory
        {
            m_Offset = m_Key.selectedKey.compiledData.memoryOffset,
            m_IntegerKey = m_Key.selectedKey.keyType == BlackboardKeyType.Integer,
            m_ComparisonType = m_ComparisonType,
            m_CompareTo = m_Value,
            m_Tolerance = m_EqualityTolerance
        };

        // actual function
        [BurstCompile(DisableDirectCall = true), MonoPInvokeCallback(typeof(DecoratorDelegate))]
        private static bool ActualFunction(in LowLevelBlackboard blackboard, byte* rawMemory)
        {
            var memory = (Memory*) rawMemory;
            var blackboardValue = memory->m_IntegerKey
                ? blackboard.Access<int>(memory->m_Offset)
                : blackboard.Access<float>(memory->m_Offset);

            var compareTo = memory->m_CompareTo;

            switch (memory->m_ComparisonType)
            {
                case ComparisonType.AlmostEqualTo:
                    return math.abs(blackboardValue - compareTo) <= memory->m_Tolerance;
                case ComparisonType.GreaterThan:
                    return blackboardValue > compareTo;
                case ComparisonType.GreaterThanEqualTo:
                    return blackboardValue >= compareTo;
                case ComparisonType.LesserThan:
                    return blackboardValue < compareTo;
                case ComparisonType.LesserThanEqualTo:
                    return blackboardValue <= compareTo;
                default:
                    throw new System.ArgumentOutOfRangeException($"Unknown comparison type: {memory->m_ComparisonType}");
            }
        }
    }
}