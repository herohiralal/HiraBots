#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    internal partial struct LGOAPGoalLayer
    {
        internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            foreach (var goal in m_Goals)
            {
                goal.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            }
        }
    }

    internal partial struct LGOAPTaskLayer
    {
        internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            foreach (var task in m_Tasks)
            {
                task.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            }
        }
    }

    internal partial class LGOAPDomain
    {
        private void OnValidate()
        {
            var keys = new HashSet<BlackboardKey>();
            if (m_Blackboard != null)
            {
                m_Blackboard.GetKeySet(keys);
            }

            var readOnlyKeySet = keys.ReadOnly();

            m_TopLayer.OnTargetBlackboardTemplateChanged(m_Blackboard, readOnlyKeySet);

            foreach (var layer in m_IntermediateLayers)
            {
                layer.OnTargetBlackboardTemplateChanged(m_Blackboard, readOnlyKeySet);
            }

            m_BottomLayer.OnTargetBlackboardTemplateChanged(m_Blackboard, readOnlyKeySet);
        }
    }
}
#endif