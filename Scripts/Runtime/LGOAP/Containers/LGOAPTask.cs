using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPTask : ScriptableObject
    {
        [SerializeField, HideInInspector] private LGOAPAction m_Action = LGOAPAction.empty;
        internal ref LGOAPAction action => ref m_Action;

        [Tooltip("Whether the given task is an abstract task and provides a long-term direction instead of executing.")]
        [SerializeField, HideInInspector] private bool m_IsAbstract = false;
        internal ref bool isAbstract => ref m_IsAbstract;

        [SerializeField, HideInInspector] private LGOAPTarget m_Target = LGOAPTarget.empty;
        internal ref LGOAPTarget target => ref m_Target;
    }
}