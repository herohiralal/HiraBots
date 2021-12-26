#if UNITY_EDITOR
using UnityEngine;

namespace HiraBots
{
    internal partial class ErrorTaskProvider
    {
        protected override void UpdateDescription(out string staticDescription)
        {
            staticDescription = m_DisablePlayModeEntryOrBuildingPlayer
                ? "Disable play mode entry or building a player and log the error."
                : "Log the error once upon Begin() and then fail the execution.";
        }
    }

    internal partial class WaitBlackboardTimeTaskProvider
    {
        protected override void OnValidateCallback()
        {
            m_Timer.keyTypesFilter = UnityEngine.BlackboardKeyType.Numeric;
        }

        protected override void OnTargetBlackboardTemplateChanged(UnityEngine.BlackboardTemplate template, in UnityEngine.BlackboardTemplate.KeySet keySet)
        {
            m_Timer.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            var key = m_Timer.selectedKey;

            staticDescription = key.isValid
                ? $"Wait for [{key.name}] second(s)."
                : "[CONTAINS ERRORS]";
        }
    }

    internal partial class WaitTaskProvider
    {
        protected override void OnValidateCallback()
        {
            m_Timer = Mathf.Max(0, m_Timer);
            m_RandomDeviation = Mathf.Max(0, m_RandomDeviation);
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            staticDescription = m_RandomDeviation == 0
                ? $"Wait for {m_Timer} second(s)."
                : $"Wait for {m_Timer} ± {m_RandomDeviation} second(s).";
        }
    }
}
#endif