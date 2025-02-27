﻿#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System;

namespace HiraBots
{
    internal partial class ErrorServiceProvider
    {
        protected override void Validate(Action<string> reportError, in UnityEngine.AI.BlackboardTemplate.KeySet keySet)
        {
            if (m_DisablePlayModeEntryOrBuildingPlayer)
            {
                reportError($"{name} could not be compiled because <i>{m_Text}</i>");
            }
        }
    }
}
#endif