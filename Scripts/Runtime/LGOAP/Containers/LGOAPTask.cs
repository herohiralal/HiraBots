using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPTask : ScriptableObject
    {
        [SerializeField, HideInInspector] private LGOAPAction m_Action = LGOAPAction.empty;
        internal ref LGOAPAction action => ref m_Action;

        [SerializeField, HideInInspector] private LGOAPTarget m_Target = LGOAPTarget.empty;
        internal ref LGOAPTarget target => ref m_Target;

        [SerializeField, HideInInspector] internal HiraBotsTaskProvider m_TaskProvider = null;
        internal ref HiraBotsTaskProvider taskProvider => ref m_TaskProvider;

        internal bool isAbstract => m_TaskProvider == null;

        [SerializeField, HideInInspector] internal HiraBotsServiceProvider[] m_ServiceProviders = new HiraBotsServiceProvider[0];
        internal ref HiraBotsServiceProvider[] serviceProviders => ref m_ServiceProviders;
    }
}