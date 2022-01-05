using System.Collections.Generic;
using HiraBots;

namespace UnityEngine
{
    [DefaultExecutionOrder(950)]
    [AddComponentMenu("AI/HiraBot (LGOAP realtime)")]
    public sealed partial class HiraLGOAPRealtimeBot : MonoBehaviour, IHiraBotArchetype
    {
        internal static readonly List<HiraLGOAPRealtimeBot> s_ActiveBots = new List<HiraLGOAPRealtimeBot>();

        [Tooltip("The component to use as an archetype. If not provided, will use self.")]
        [SerializeField, HideInInspector] private Component m_ArchetypeOverride = null;

        [Tooltip("The domain to use for this HiraBot.")]
        [SerializeField, HideInInspector] private HiraBots.LGOAPDomain m_Domain = null;

        [SerializeField, HideInInspector] private LGOAPRealtimeBotComponent m_Internal = new LGOAPRealtimeBotComponent(null);

        [Tooltip("Whether to run the planner synchronously and on the main thread.")]
        [SerializeField] private bool m_RunPlannerSynchronously = false;

        /// <summary>
        /// "The component to use as an archetype. If not provided, will use self."
        /// </summary>
        public Component archetypeOverride
        {
            get => m_ArchetypeOverride;
            set
            {
                if (value is IHiraBotArchetype arch)
                {
                    m_ArchetypeOverride = value;
                    m_Internal.m_EffectiveArchetype = arch;
                }
                else
                {
                    if (value != null)
                    {
                        Debug.LogWarning($"Unsupported archetype: \"{value}\". Using self as fallback.", this);
                    }

                    m_ArchetypeOverride = null;
                    m_Internal.m_EffectiveArchetype = this;
                }
            }
        }

        /// <summary>
        /// The domain to use for this HiraBot.
        /// </summary>
        internal LGOAPDomain domain
        {
            get => m_Domain;
            set => m_Domain = value.m_Value;
        }

        /// <summary>
        /// The multiplier to use on tick interval of a task/service. Local to this HiraBot. Can be used for LOD purposes.
        /// </summary>
        internal float executableTickIntervalMultiplier
        {
            get => m_Internal.m_ExecutableTickIntervalMultiplier;
            set
            {
                m_Internal.executableTickIntervalMultiplier = value;
                m_Internal.m_ExecutableTickIntervalMultiplier = value;
            }
        }

        /// <summary>
        /// Whether to run the planner synchronously and on the main thread.
        /// </summary>
        internal bool runPlannerSynchronously
        {
            get => m_RunPlannerSynchronously;
            set
            {
                m_Internal.runPlannerSynchronously = value;
                m_RunPlannerSynchronously = value;
            }
        }

        /// <summary>
        /// Update the status of this HiraBot. Triggers rechecking blackboard for any changes essential to
        /// decision-making, or updates the current task if there is one available but not being used.
        /// </summary>
        internal void UpdateStatus()
        {
            Update();
        }

        private void Awake()
        {
            s_ActiveBots.Add(this);
            StartUsingNewDomain();
        }

        internal void Dispose()
        {
            StopUsingOldDomain();
            s_ActiveBots.Remove(this);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void OnEnable()
        {
            m_Internal.executableTickPaused = false;
        }

        private void OnDisable()
        {
            m_Internal.executableTickPaused = true;
        }

        private void StartUsingNewDomain()
        {
            m_Internal = new LGOAPRealtimeBotComponent(m_Domain)
            {
                m_EffectiveArchetype = m_ArchetypeOverride is IHiraBotArchetype arch ? arch : this,
                runPlannerSynchronously = m_RunPlannerSynchronously
            };
        }

        private void StopUsingOldDomain()
        {
            m_Internal.Dispose();
            m_Internal = new LGOAPRealtimeBotComponent(null);
        }

        private void Update()
        {
            if (!ReferenceEquals(m_Domain, m_Internal.m_Domain))
            {
                StopUsingOldDomain();
                StartUsingNewDomain();
            }

            m_Internal.Tick();
        }
    }
}