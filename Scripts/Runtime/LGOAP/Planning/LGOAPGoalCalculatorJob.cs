using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace HiraBots
{
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
    internal unsafe struct LGOAPGoalCalculatorJob : IJob
    {
        public LGOAPGoalCalculatorJob(NativeArray<byte> domain, NativeArray<byte> blackboard, PlannerResult result)
        {
            m_Domain = domain;
            m_Blackboard = blackboard;
            m_Result = result;
        }

        [ReadOnly] private readonly NativeArray<byte> m_Domain;
        [ReadOnly] private readonly NativeArray<byte> m_Blackboard;
        private PlannerResult m_Result;

        public void Execute()
        {
            var domain = new LowLevelLGOAPDomain((byte*) m_Domain.GetUnsafeReadOnlyPtr());
            var blackboard = new LowLevelBlackboard((byte*) m_Blackboard.GetUnsafeReadOnlyPtr(), (ushort) m_Blackboard.Length);

            var answer = domain.insistenceCollection.GetMaxInsistenceIndex(blackboard);
            answer = answer == -1 ? 0 : answer;

            m_Result.count = 1;

            m_Result.resultType = m_Result.resultType != PlannerResult.Type.Invalid && answer == m_Result[0]
                    ? PlannerResult.Type.Unchanged
                    : PlannerResult.Type.NewPlan;

            m_Result[0] = (short) answer;

            m_Result.RestartPlan();
        }
    }
}