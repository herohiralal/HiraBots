using System;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardTemplate
    {
        /// <summary>
        /// Lets the user select a key using a dropdown, based on the applied constraints.
        /// </summary>
        [Serializable]
        internal partial struct KeySelector
        {
            [SerializeField, HideInInspector] private BlackboardKey m_Key;

#if !(UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER)
            internal void UpdateFilter(BlackboardTemplate _)
            {
                // intentionally unused
            }

            internal void UpdateFilter(BlackboardKeyType _)
            {
                // intentionally unused
            }
#else
            [SerializeField, HideInInspector] private BlackboardTemplate m_Template;
            [SerializeField, HideInInspector] private BlackboardKeyType m_KeyTypes;

            /// <summary>
            /// Update the template filter.
            /// </summary>
            internal void UpdateFilter(BlackboardTemplate template)
            {
                m_Template = template;
            }

            /// <summary>
            /// Update the key types filter.
            /// </summary>
            internal void UpdateFilter(BlackboardKeyType keyTypes)
            {
                m_KeyTypes = keyTypes;
            }
#endif
        }
    }
}