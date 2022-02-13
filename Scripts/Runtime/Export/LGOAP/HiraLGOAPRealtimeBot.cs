using System.Collections.Generic;
using HiraBots;

namespace UnityEngine.AI
{
    [AddComponentMenu("AI/HiraBot (LGOAP realtime)")]
    public sealed partial class HiraLGOAPRealtimeBot : MonoBehaviour, IHiraBotArchetype, IUpdatableBehaviour
    {
        internal static readonly List<HiraLGOAPRealtimeBot> s_ActiveBots = new List<HiraLGOAPRealtimeBot>();

        [Tooltip("The component to use as an archetype. If not provided, will use self.")]
        [SerializeField, HideInInspector] private Component m_ArchetypeOverride = null;

        [Tooltip("The domain to use for this HiraBot.")]
        [SerializeField, HideInInspector] private HiraBots.LGOAPDomain m_Domain = null;

        [Tooltip("The ticking interval to check for blackboard updates or task updates. Negative value means no auto-update.")]
        [SerializeField, HideInInspector] private float m_TickInterval = 0f;

        [Tooltip("The multiplier to use on tick interval of a task/service. Local to this bot. Can be used for LOD purposes.")]
        [SerializeField] private float m_ExecutableTickIntervalMultiplier = 1f;

        [Tooltip("Whether to run the planner synchronously and on the main thread.")]
        [SerializeField] private bool m_RunPlannerSynchronously = false;

        [SerializeField, HideInInspector] private LGOAPRealtimeBotComponent m_Internal = new LGOAPRealtimeBotComponent(null);

        [System.NonSerialized] private bool m_Disposed;

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
        public LGOAPDomain domain
        {
            get => m_Domain;
            set => m_Domain = value.m_Value;
        }

        /// <summary>
        /// The blackboard component currently in use by this HiraBot.
        /// </summary>
        public BlackboardComponent blackboardComponent => m_Internal.m_Blackboard;

        /// <summary>
        /// The ticking interval to check for blackboard updates or task updates. Negative value means no auto-update.
        /// </summary>
        public float tickInterval
        {
            get => m_TickInterval;
            set
            {
                m_TickInterval = value;

                if (!isActiveAndEnabled || string.IsNullOrWhiteSpace(gameObject.scene.path))
                {
                    return;
                }

                if (value < 0f)
                {
                    BehaviourUpdater.Remove(this);
                }
                else
                {
                    BehaviourUpdater.Add(this, value);
                }
            }
        }

        /// <summary>
        /// The multiplier to use on tick interval of a task/service. Local to this HiraBot. Can be used for LOD purposes.
        /// </summary>
        public float executableTickIntervalMultiplier
        {
            get => m_ExecutableTickIntervalMultiplier;
            set
            {
                m_ExecutableTickIntervalMultiplier = value;
                m_Internal.executableTickIntervalMultiplier = value;
            }
        }

        /// <summary>
        /// Whether to run the planner synchronously and on the main thread.
        /// </summary>
        public bool runPlannerSynchronously
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
        public void Tick()
        {
            if (!ReferenceEquals(m_Domain, m_Internal.m_Domain))
            {
                StopUsingOldDomain();
                StartUsingNewDomain();
            }

            m_Internal.Tick();
        }

        /// <summary>
        /// Send a message to all the tasks/services currently being executed.
        /// </summary>
        public void Message<T>(T message)
        {
            m_Internal.Message(message);
        }

        private void Awake()
        {
            m_Disposed = false;

            s_ActiveBots.Add(this);
            StartUsingNewDomain();
        }

        internal void Dispose()
        {
            StopUsingOldDomain();
            m_Disposed = true;
        }

        private void OnDestroy()
        {
            if (!m_Disposed)
            {
                Dispose();
            }

            s_ActiveBots.Remove(this);
        }

        private void OnEnable()
        {
            if (m_TickInterval >= 0f)
            {
                BehaviourUpdater.Add(this, m_TickInterval);
            }

            m_Internal.executableTickPaused = false;
        }

        private void OnDisable()
        {
            if (!m_Disposed)
            {
                m_Internal.executableTickPaused = true;

                BehaviourUpdater.Remove(this);
            }
        }

        private void StartUsingNewDomain()
        {
            m_Internal = new LGOAPRealtimeBotComponent(m_Domain)
            {
                m_EffectiveArchetype = m_ArchetypeOverride is IHiraBotArchetype arch ? arch : this,
                executableTickIntervalMultiplier = m_ExecutableTickIntervalMultiplier,
                runPlannerSynchronously = m_RunPlannerSynchronously
            };
        }

        private void StopUsingOldDomain()
        {
            m_Internal.Dispose();
            m_Internal = new LGOAPRealtimeBotComponent(null);
        }

        void IUpdatableBehaviour.Tick(float _)
        {
            Tick();
        }
    }
}