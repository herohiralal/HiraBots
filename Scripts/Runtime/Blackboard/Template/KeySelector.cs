﻿using System;
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

            /// <summary>
            /// The currently selected key.
            /// </summary>
            internal BlackboardKey selectedKey
            {
                get => m_Key;
                set => m_Key = value;
            }

            /// <summary>
            /// Implicitly convert a KeySelector to its public interface.
            /// </summary>
            public static implicit operator UnityEngine.AI.BlackboardTemplate.KeySelector(KeySelector selector)
            {
                return new UnityEngine.AI.BlackboardTemplate.KeySelector(selector);
            }

#if UNITY_EDITOR
#pragma warning disable CS0414
            // filter data is only needed in the editor - the actual validation occurs based on
            // the data provided by the owning object

            [SerializeField, HideInInspector] private BlackboardTemplate m_Template;
            [SerializeField, HideInInspector] private BlackboardKeyType m_KeyTypes;
            [SerializeField, HideInInspector] private bool m_IsValid;

#pragma warning restore CS0414

            /// <summary>
            /// Update the key types filter.
            /// </summary>
            internal BlackboardKeyType keyTypesFilter
            {
                set
                {
                    m_KeyTypes = value;

                    if (m_Key == null || !value.HasFlag(m_Key.keyType))
                    {
                        m_IsValid = false;
                    }
                }
            }

            /// <summary>
            /// Update the template filter and pass in the set of keys associated with it.
            /// </summary>
            internal void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, ReadOnlyHashSetAccessor<BlackboardKey> keySet)
            {
                m_Template = newTemplate;

                if (m_Key == null || (newTemplate != null && !keySet.Contains(m_Key)))
                {
                    m_IsValid = false;
                }
            }
#endif
        }
    }
}