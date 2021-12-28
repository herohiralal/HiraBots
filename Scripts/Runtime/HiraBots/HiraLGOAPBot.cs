using UnityEngine;

namespace HiraBots
{
    internal sealed partial class HiraLGOAPBot : MonoBehaviour, IHiraBotArchetype
    {
        [Tooltip("The component to use as an archetype. If not provided, will use self.")]
        [SerializeField] private Component m_ArchetypeOverride;

        [Tooltip("The domain to use for this HiraBot.")]
        [SerializeField] private LGOAPDomain m_Domain;

        private void OnValidate()
        {
            if (!(m_ArchetypeOverride is IHiraBotArchetype))
            {
                m_ArchetypeOverride = null;
            }
        }

        private LGOAPDomain m_DomainCurrentlyInUse = null;

        private BlackboardComponent m_Blackboard = null;
        private LGOAPPlannerComponent m_Planner = null;
        private ExecutorComponent m_Executor = null;

        private void Awake()
        {
            StartUsingNewDomain();
        }

        private void OnDestroy()
        {
            StopUsingOldDomain();
        }

        private void Update()
        {
            if (!ReferenceEquals(m_Domain, m_DomainCurrentlyInUse))
            {
                StopUsingOldDomain();
                StartUsingNewDomain();
            }

            if (ReferenceEquals(m_DomainCurrentlyInUse, null))
            {
                return;
            }

            if (m_Blackboard.hasUnexpectedChanges)
            {
                var changes = "unexpected changes: ";
                for (var i = 0; i < m_Blackboard.unexpectedChanges.count; i++)
                {
                    changes += $"[{m_Blackboard.unexpectedChanges[i]}] ";
                }

                Debug.Log(changes);

                m_Blackboard.ClearUnexpectedChanges();
            }
        }

        private void StartUsingNewDomain()
        {
            if (ReferenceEquals(m_Domain, null) || !m_Domain.isCompiled)
            {
                return;
            }

            if (!BlackboardComponent.TryCreate(m_Domain.compiledData.blackboardTemplate, out m_Blackboard))
            {
                return;
            }

            if (!LGOAPPlannerComponent.TryCreate(m_Blackboard, m_Domain.compiledData, out m_Planner))
            {
                m_Blackboard.Dispose();
                m_Blackboard = null;

                return;
            }

            if (!ExecutorComponent.TryCreate(out m_Executor))
            {
                m_Planner.Dispose();
                m_Planner = null;

                m_Blackboard.Dispose();
                m_Blackboard = null;

                return;
            }

            m_DomainCurrentlyInUse = m_Domain;
        }

        private void StopUsingOldDomain()
        {
            if (ReferenceEquals(m_DomainCurrentlyInUse, null))
            {
                return;
            }

            m_Executor.Dispose();
            m_Executor = null;

            m_Planner.Dispose();
            m_Planner = null;

            m_Blackboard.Dispose();
            m_Blackboard = null;

            m_DomainCurrentlyInUse = null;
        }
    }
}