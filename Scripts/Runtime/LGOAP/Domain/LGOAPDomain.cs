using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPDomain : ScriptableObject
    {
        [Tooltip("The backends to use.")]
        [SerializeField, HideInInspector] private BackendType m_Backends = BackendType.RuntimeInterpreter;

        /// <summary>
        /// The effective backend to use.
        /// </summary>
        internal BackendType backends => m_Backends;

        [Tooltip("The blackboard to use for the domain.")]
        [SerializeField, HideInInspector] private BlackboardTemplate m_Blackboard = null;

        [Tooltip("The top-most layer, containing only goals.")]
        [SerializeField, HideInInspector] private LGOAPGoalLayer m_TopLayer = LGOAPGoalLayer.empty;

        [Tooltip("The intermediate layers, containing abstract tasks.")]
        [SerializeField, HideInInspector] private LGOAPTaskLayer[] m_IntermediateLayers = new LGOAPTaskLayer[0];

        [Tooltip("The bottom-most layer, containing only executable tasks.")]
        [SerializeField, HideInInspector] private LGOAPTaskLayer m_BottomLayer = LGOAPTaskLayer.empty;

        /// <summary>
        /// The blackboard to use for this domain.
        /// </summary>
        internal BlackboardTemplate blackboard => m_Blackboard;

        /// <summary>
        /// Implicitly convert an LGOAPDomain to its public interface.
        /// </summary>
        public static implicit operator UnityEngine.LGOAPDomain(LGOAPDomain actualTemplate)
        {
            return new UnityEngine.LGOAPDomain(actualTemplate);
        }
    }
}