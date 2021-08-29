#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPGoal
    {
        internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            m_Insistence.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            m_Target.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }

    internal partial class LGOAPTask
    {
        internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            m_Action.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            m_Target.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
        }
    }
}
#endif