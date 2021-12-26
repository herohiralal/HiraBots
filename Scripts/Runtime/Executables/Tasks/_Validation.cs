#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System;

namespace HiraBots
{
    internal partial class ErrorTaskProvider
    {
        protected override void Validate(Action<string> reportError)
        {
            if (m_DisablePlayModeEntryOrBuildingPlayer)
            {
                reportError($"{name} could not be compiled because <i>{m_Text}</i>");
            }
        }
    }
}
#endif