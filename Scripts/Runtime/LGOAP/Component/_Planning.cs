using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace HiraBots
{
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
    internal unsafe struct LGOAPGoalCalculatorJob : IJob
    {
        public LGOAPGoalCalculatorJob(NativeArray<byte> domain, NativeArray<byte> blackboard, LGOAPPlan result)
        {
            m_Domain = domain;
            m_Blackboard = blackboard;
            m_Result = result;
        }

        [ReadOnly] private readonly NativeArray<byte> m_Domain;
        [ReadOnly] private readonly NativeArray<byte> m_Blackboard;
        private LGOAPPlan m_Result;

        public void Execute()
        {
            var domain = new LowLevelLGOAPDomain((byte*) m_Domain.GetUnsafeReadOnlyPtr());
            var blackboard = new LowLevelBlackboard((byte*) m_Blackboard.GetUnsafeReadOnlyPtr(), (ushort) m_Blackboard.Length);

            var answer = domain.insistenceCollection.GetMaxInsistenceIndex(blackboard);
            answer = answer == -1 ? 0 : answer;

            m_Result.length = 1;

            m_Result.resultType = m_Result.resultType != LGOAPPlan.Type.Invalid && answer == m_Result[0]
                ? LGOAPPlan.Type.Unchanged
                : LGOAPPlan.Type.NewPlan;

            m_Result[0] = (short) answer;

            m_Result.RestartPlan();
        }
    }

    [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
    internal unsafe struct LGOAPMainPlannerJob : IJob
    {
        public LGOAPMainPlannerJob(NativeArray<byte> domain,
            NativeArray<byte> blackboard,
            float maxFScore,
            int layerIndex,
            LGOAPPlan previousLayerResult,
            LGOAPPlan result)
        {
            m_Domain = domain;
            m_Blackboard = blackboard;
            m_MaxFScore = maxFScore;
            m_LayerIndex = layerIndex;
            m_PreviousLayerResult = previousLayerResult;
            m_Result = result;
        }

        [ReadOnly] private readonly NativeArray<byte> m_Domain;
        [ReadOnly] private readonly NativeArray<byte> m_Blackboard;

        private readonly float m_MaxFScore;
        private readonly int m_LayerIndex;
        private LGOAPPlan m_PreviousLayerResult;
        private LGOAPPlan m_Result;

        public void Execute()
        {
            if (m_PreviousLayerResult.resultType == LGOAPPlan.Type.Invalid)
            {
                m_Result.InvalidatePlan();
                return;
            }

            var datasetsPtr = stackalloc byte[m_Blackboard.Length * (m_Result.maxLength + 1)];
            var datasets = new LowLevelBlackboardCollection(datasetsPtr, (ushort) (m_Result.maxLength + 1), (ushort) m_Blackboard.Length);
            datasets.Copy(0, m_Blackboard);

            var domain = new LowLevelLGOAPDomain((byte*) m_Domain.GetUnsafeReadOnlyPtr());
            var (targets, actions) = domain.GetTaskLayerAt(m_LayerIndex);

            var target = targets[m_PreviousLayerResult[0]];

            // check if the target is fake
            if (target.isFake)
            {
                m_Result.InvalidatePlan();
                m_Result.resultType = LGOAPPlan.Type.NotRequired;
                return;
            }

            // check if the current plan is ok
            if (m_PreviousLayerResult.resultType == LGOAPPlan.Type.Unchanged && m_Result.resultType != LGOAPPlan.Type.Invalid)
            {
                var previousPlanStillValid = true;

                // check if the actions in the plan can still be chained
                var planLength = m_Result.length;
                for (var i = m_Result.currentIndex; i < planLength; i++)
                {
                    actions.collection[m_Result[i]].Break(
                        out var precondition,
                        out _,
                        out var effect);

                    if (!precondition.Execute(datasets[0]))
                    {
                        previousPlanStillValid = false;
                        break;
                    }

                    effect.Execute(datasets[0]);
                }

                // current plan is still valid AND it satisfies the target
                if (previousPlanStillValid && target.GetHeuristic(datasets[0]) == 0)
                {
                    m_Result.resultType = LGOAPPlan.Type.Unchanged;
                    return;
                }

                datasets.Copy(0, m_Blackboard);
            }

            new NewPlanFinder
            {
                m_Goal = target,
                m_Actions = actions,
                m_MaxFScore = m_MaxFScore,
                m_Datasets = datasets,
                m_Result = m_Result
            }.Execute();
        }

        private struct NewPlanFinder
        {
            internal LowLevelLGOAPTarget m_Goal;
            internal LowLevelLGOAPActionCollection m_Actions;
            internal float m_MaxFScore;
            internal LowLevelBlackboardCollection m_Datasets;
            internal LGOAPPlan m_Result;

            internal void Execute()
            {
                m_Result.InvalidatePlan();

                float threshold = m_Goal.GetHeuristic(m_Datasets[0]);
                float score;
                while ((score = PerformHeuristicEstimatedSearch(1, 0, threshold)) > 0 && score <= m_MaxFScore)
                {
                    threshold = score;
                }
            }

            private float PerformHeuristicEstimatedSearch(byte index, float costUntilNow, float threshold)
            {
                var heuristic = m_Goal.GetHeuristic(m_Datasets[index - 1]);

                var fScore = costUntilNow + heuristic;
                if (fScore > threshold)
                {
                    return fScore;
                }

                if (heuristic == 0)
                {
                    m_Result.RestartPlan();
                    m_Result.resultType = LGOAPPlan.Type.NewPlan;
                    m_Result.length = (short) (index - 1);
                    return -1;
                }

                if (index == m_Datasets.count)
                {
                    return float.MaxValue;
                }

                var min = float.MaxValue;

                var iterator = m_Actions.GetEnumerator();
                while (iterator.MoveNext())
                {
                    iterator.current.Break(
                        out var precondition,
                        out var cost,
                        out var effect);

                    if (!precondition.Execute(m_Datasets[index - 1]))
                    {
                        continue;
                    }

                    var currentCost = costUntilNow + cost.Execute(m_Datasets[index - 1]);

                    m_Datasets.Copy(index, index - 1);

                    effect.Execute(m_Datasets[index]);

                    float score;
                    if ((score = PerformHeuristicEstimatedSearch((byte) (index + 1), currentCost, threshold)) < 0)
                    {
                        m_Result[(short) (index - 1)] = (short) iterator.currentIndex;
                        return -1;
                    }

                    min = math.min(score, min);
                }

                return min;
            }
        }
    }
}