using System;
using UnityEngine;

namespace HiraBots
{
    internal partial class BlackboardKey
    {
        /// <summary>
        /// Lets the user select a key using a dropdown, based on the applied constraints.
        /// </summary>
        [Serializable]
        internal partial struct Selector
        {
            [SerializeField, HideInInspector] private BlackboardKey m_Key;

            /// <summary>
            /// The currently selected key.
            /// </summary>
            internal BlackboardKey selectedKey
            {
                get => m_Key;
                set => m_Key = value;
            }

#if UNITY_EDITOR
            // filter data is only needed in the editor - the actual validation occurs based on
            // the data provided by the owning object

            [SerializeField, HideInInspector] private BlackboardTemplate m_Template;
            [SerializeField, HideInInspector] private BlackboardKeyType m_KeyTypes;

            /// <summary>
            /// Update the template filter.
            /// </summary>
            internal BlackboardTemplate templateFilter
            {
                set => m_Template = value;
            }

            /// <summary>
            /// Update the key types filter.
            /// </summary>
            internal BlackboardKeyType keyTypesFilter
            {
                set => m_KeyTypes = value;
            }
#else
            internal BlackboardTemplate templateFilter
            {
                set { }
            }

            internal BlackboardKeyType keyTypesFilter
            {
                set { }
            }
#endif
        }
    }
}