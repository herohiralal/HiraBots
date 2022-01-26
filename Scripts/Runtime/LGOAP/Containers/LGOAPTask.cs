using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal partial class LGOAPTask : ScriptableObject
    {
        [SerializeField, HideInInspector] private LGOAPAction m_Action = LGOAPAction.empty;
        internal ref LGOAPAction action => ref m_Action;

        [SerializeField, HideInInspector] private LGOAPTarget m_Target = LGOAPTarget.empty;
        internal ref LGOAPTarget target => ref m_Target;

        [SerializeField, HideInInspector] private HiraBotsTaskProvider[] m_TaskProviders = new HiraBotsTaskProvider[0];
        internal ref HiraBotsTaskProvider[] taskProviders => ref m_TaskProviders;

        internal bool isAbstract => m_TaskProviders.Length == 0;

        [SerializeField, HideInInspector] private HiraBotsServiceProvider[] m_ServiceProviders = new HiraBotsServiceProvider[0];
        internal ref HiraBotsServiceProvider[] serviceProviders => ref m_ServiceProviders;
    }
}