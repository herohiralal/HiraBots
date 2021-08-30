﻿using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPDomain : ScriptableObject
    {
        [Tooltip("The backends to use.")]
        [SerializeField, HideInInspector] private BackendType m_Backends = BackendType.RuntimeInterpreter;

        [Tooltip("The blackboard to use for the domain.")]
        [SerializeField, HideInInspector] private BlackboardTemplate m_Blackboard = null;

        [Tooltip("The top-most layer, containing only goals.")]
        [SerializeField, HideInInspector] private LGOAPGoalLayer m_TopLayer = LGOAPGoalLayer.empty;

        [Tooltip("The intermediate layers, containing abstract tasks.")]
        [SerializeField, HideInInspector] private LGOAPTaskLayer[] m_IntermediateLayers = new LGOAPTaskLayer[0];

        [Tooltip("The bottom-most layer, containing only executable tasks.")]
        [SerializeField, HideInInspector] private LGOAPTaskLayer m_BottomLayer = LGOAPTaskLayer.empty;
    }
}