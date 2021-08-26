using UnityEngine;

namespace HiraBots
{
    internal sealed partial class LGOAPGoal : ScriptableObject
    {
        [SerializeField, HideInInspector] private LGOAPInsistence m_Insistence = LGOAPInsistence.empty;
        internal ref LGOAPInsistence insistence => ref m_Insistence;

        [SerializeField, HideInInspector] private LGOAPTarget m_Target = LGOAPTarget.empty;
        internal ref LGOAPTarget target => ref m_Target;

        internal void PrepareForCompilation()
        {
            m_Insistence.PrepareForCompilation();
            m_Target.PrepareForCompilation(false);
        }
    }
}