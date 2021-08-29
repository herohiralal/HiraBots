using UnityEngine;

namespace HiraBots
{
    [CreateAssetMenu]
    internal partial class LGOAPDomain : ScriptableObject
    {
        [Tooltip("The blackboard to use for the domain.")]
        [SerializeField, HideInInspector] private BlackboardTemplate m_Blackboard = null;

        [Tooltip("The top-most layer, containing only goals.")]
        [SerializeField] private LGOAPGoalLayer m_TopLayer = LGOAPGoalLayer.empty;

        [Tooltip("The intermediate layers, containing abstract tasks.")]
        [SerializeField] private LGOAPTaskLayer[] m_IntermediateLayers = new LGOAPTaskLayer[0];

        [Tooltip("The bottom-most layer, containing only executable tasks.")]
        [SerializeField] private LGOAPTaskLayer m_BottomLayer = LGOAPTaskLayer.empty;
    }
}