using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace HiraBots
{
    [BurstCompile]
    internal unsafe partial class LGOAPPlannerComponent
    {
        [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
        private struct LGOAPGoalCalculatorJob : IJob
        {
            internal LGOAPGoalCalculatorJob(NativeArray<byte> domain,
                NativeArray<byte> blackboard,
                LGOAPPlan.ReadOnly fallbackPlan,
                LGOAPPlan result)
            {
                m_Domain = domain;
                m_Blackboard = blackboard;
                m_FallbackPlan = fallbackPlan;
                m_Result = result;
            }

            [ReadOnly] private readonly NativeArray<byte> m_Domain;
            [ReadOnly] private readonly NativeArray<byte> m_Blackboard;
            [ReadOnly] private readonly LGOAPPlan.ReadOnly m_FallbackPlan;
            private LGOAPPlan m_Result;

            public void Execute()
            {
                var domain = new LowLevelLGOAPDomain((byte*) m_Domain.GetUnsafeReadOnlyPtr());
                var blackboard = new LowLevelBlackboard((byte*) m_Blackboard.GetUnsafeReadOnlyPtr(), (ushort) m_Blackboard.Length);

                var answer = domain.insistenceCollection.GetMaxInsistenceIndex(blackboard);

                if (answer == -1)
                {
                    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                    m_FallbackPlan.CopyTo(m_Result);
                }
                else
                {
                    m_Result.length = 1;

                    // if the goal has been achieved, need to mark it as new plan
                    // or it won't get properly picked up
                    m_Result.resultType = answer == m_Result[0] && m_Result.currentIndex == 0 ? LGOAPPlan.Type.Unchanged : LGOAPPlan.Type.NewPlan;

                    m_Result[0] = (short) answer;

                    m_Result.currentIndex = 0;
                }
            }
        }

        [BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
        private struct LGOAPMainPlannerJob : IJob
        {
            internal LGOAPMainPlannerJob(NativeArray<byte> domain,
                NativeArray<byte> blackboard,
                LGOAPPlan.ReadOnly fallbackPlan,
                float maxFScore,
                int layerIndex,
                LGOAPPlan previousLayerResult,
                LGOAPPlan result)
            {
                m_Domain = domain;
                m_Blackboard = blackboard;
                m_FallbackPlan = fallbackPlan;
                m_MaxFScore = maxFScore;
                m_LayerIndex = layerIndex;
                m_PreviousLayerResult = previousLayerResult;
                m_Result = result;
            }

            [ReadOnly] private readonly NativeArray<byte> m_Domain;
            [ReadOnly] private readonly NativeArray<byte> m_Blackboard;
            [ReadOnly] private readonly LGOAPPlan.ReadOnly m_FallbackPlan;

            private readonly float m_MaxFScore;
            private readonly int m_LayerIndex;
            private LGOAPPlan m_PreviousLayerResult;
            private LGOAPPlan m_Result;

            public void Execute()
            {
                if (m_PreviousLayerResult.resultType == LGOAPPlan.Type.NotRequired)
                {
                    m_Result.length = 0;
                    m_Result.currentIndex = 0;
                    m_Result.resultType = LGOAPPlan.Type.NotRequired;
                    return;
                }

                var domain = new LowLevelLGOAPDomain((byte*) m_Domain.GetUnsafeReadOnlyPtr());
                var (targets, actions) = domain.GetTaskLayerAt(m_LayerIndex);

                var target = targets[m_PreviousLayerResult[m_PreviousLayerResult.currentIndex]];

                // check if the target is fake
                if (target.isFake)
                {
                    m_Result.length = 0;
                    m_Result.currentIndex = 0;
                    m_Result.resultType = LGOAPPlan.Type.NotRequired;
                    return;
                }

                var datasetsPtr = (byte*) UnsafeUtility.Malloc(
                    m_Blackboard.Length * (m_Result.maxLength + 1),
                    UnsafeUtility.AlignOf<byte>(),
                    Allocator.Persistent);

                var datasets = new LowLevelBlackboardCollection(datasetsPtr, (ushort) (m_Result.maxLength + 1), (ushort) m_Blackboard.Length);
                datasets.Copy(0, m_Blackboard);

                // check if the current plan is ok
                if (m_PreviousLayerResult.resultType == LGOAPPlan.Type.Unchanged)
                {
                    var previousPlanStillValid = true;

                    // check if the actions in the plan can still be chained
                    var planLength = m_Result.length;
                    var currentIndex = m_Result.currentIndex;

                    // if the plan has been used, can't use an "unchanged" plan now can we?
                    if (currentIndex < planLength)
                    {
                        for (var i = currentIndex; i < planLength; i++)
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
                }

                new NewPlanFinder
                {
                    m_Goal = target,
                    m_Actions = actions,
                    m_MaxFScore = m_MaxFScore,
                    m_Datasets = datasets,
                    m_FallbackPlan = m_FallbackPlan,
                    m_Result = m_Result
                }.Execute();

                UnsafeUtility.Free(datasetsPtr, Allocator.Persistent);
            }

            private struct NewPlanFinder
            {
                internal LowLevelLGOAPTarget m_Goal;
                internal LowLevelLGOAPActionCollection m_Actions;
                internal float m_MaxFScore;
                internal LowLevelBlackboardCollection m_Datasets;
                internal LGOAPPlan.ReadOnly m_FallbackPlan;
                internal LGOAPPlan m_Result;

                internal void Execute()
                {
                    m_FallbackPlan.CopyTo(m_Result);

                    float threshold = m_Goal.GetHeuristic(m_Datasets[0]);
                    float score;
                    while ((score = PerformHeuristicEstimatedSearch(1, 0, threshold)) > 0 && score <= m_MaxFScore)
                    {
                        threshold = score;
                    }

                    // if an empty plan was generated, i.e. the target is reached, use a fallback plan
                    if (m_Result.length == 0)
                    {
                        m_FallbackPlan.CopyTo(m_Result);
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
                        m_Result.currentIndex = 0;
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
}