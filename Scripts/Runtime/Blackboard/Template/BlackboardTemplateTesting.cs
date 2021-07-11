using UnityEngine;

#if UNITY_EDITOR && HIRA_BOTS_TESTS

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        [SerializeField, HideInInspector] private bool isTestingOnly = false;
        internal bool IsTestingOnly => isTestingOnly;
    }
}

#endif