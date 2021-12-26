#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System;

namespace HiraBots
{
    internal partial class ErrorTaskProvider
    {
        protected override void Validate(Action<string> reportError, in UnityEngine.BlackboardTemplate.KeySet keySet)
        {
            if (m_DisablePlayModeEntryOrBuildingPlayer)
            {
                reportError($"{name} could not be compiled because <i>{m_Text}</i>");
            }
        }
    }

    internal partial class WaitBlackboardTimeTaskProvider
    {
        protected override void Validate(Action<string> reportError, in UnityEngine.BlackboardTemplate.KeySet keySet)
        {
            if (!m_Timer.Validate(in keySet, UnityEngine.BlackboardKeyType.Numeric))
            {
                reportError($"{name} does not have a valid timer key.");
            }
        }
    }
}
#endif