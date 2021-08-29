#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal partial struct LGOAPInsistence
    {
        internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            foreach (var decorator in m_Insistence)
            {
                decorator.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            }
        }
    }

    internal partial struct LGOAPTarget
    {
        internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            foreach (var decorator in m_Target)
            {
                decorator.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            }
        }
    }

    internal partial struct LGOAPAction
    {
        internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
        {
            foreach (var decorator in m_Precondition)
            {
                decorator.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            }

            foreach (var decorator in m_Cost)
            {
                decorator.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            }

            foreach (var effect in m_Effect)
            {
                effect.OnTargetBlackboardTemplateChanged(newTemplate, keySet);
            }
        }
    }
}
#endif