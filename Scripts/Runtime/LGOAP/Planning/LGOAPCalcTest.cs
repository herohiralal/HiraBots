using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace HiraBots
{
    public class LGOAPCalcTest : MonoBehaviour
    {
        [SerializeField] private LGOAPDomain m_Domain;
        private BlackboardComponent m_Blackboard;
        [SerializeField] private LGOAPGoal m_Result;
        [SerializeField] private LGOAPTask[] m_Result0;
        [SerializeField] private LGOAPTask[] m_Result1;

        private void Awake()
        {
            if (!m_Domain.isCompiled || !BlackboardComponent.TryCreate(m_Domain.compiledData.blackboard, out m_Blackboard))
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            m_Blackboard = null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var domain = m_Domain.compiledData.lowLevelDomain;

                var bb = m_Blackboard.GetCopy(Allocator.TempJob);

                var goalResult = new PlannerResult(1, Allocator.TempJob);
                var layer0Result = new PlannerResult(3, Allocator.TempJob);
                var layer1Result = new PlannerResult(3, Allocator.TempJob);

                new LGOAPGoalCalculatorJob(domain, bb, goalResult).Run();
                new LGOAPMainPlannerJob(domain, bb, 200f, 0, goalResult, layer0Result).Run();
                new LGOAPMainPlannerJob(domain, bb, 200f, 1, layer0Result, layer1Result).Run();

                m_Result = null;

                while (goalResult.MoveNext())
                {
                    m_Result = m_Domain.m_TopLayer.m_Goals[goalResult.currentElement];
                }

                m_Result0 = new LGOAPTask[layer0Result.count];

                while (layer0Result.MoveNext())
                {
                    m_Result0[layer0Result.currentIndex] = m_Domain.m_IntermediateLayers[0].m_Tasks[layer0Result.currentElement];
                }

                m_Result1 = new LGOAPTask[layer1Result.count];

                while (layer1Result.MoveNext())
                {
                    m_Result1[layer1Result.currentIndex] = m_Domain.m_BottomLayer.m_Tasks[layer1Result.currentElement];
                }

                layer1Result.Dispose();
                layer0Result.Dispose();
                goalResult.Dispose();
                bb.Dispose();
            }
        }
    }
}