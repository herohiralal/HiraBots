using HiraBots;

namespace UnityEngine.AI
{
    public partial struct BlackboardTemplate
    {
        /// <summary>
        /// A blackboard template key selector.
        /// </summary>
        [System.Serializable]
        public struct KeySelector
        {
            internal KeySelector(HiraBots.BlackboardTemplate.KeySelector value)
            {
                m_Value = value;
            }

            [SerializeField, HideInInspector] internal HiraBots.BlackboardTemplate.KeySelector m_Value;

            /// <summary>
            /// The currently selected key.
            /// </summary>
            public BlackboardKey selectedKey
            {
                get => m_Value.selectedKey;
                set => m_Value.selectedKey = value.m_Value;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public BlackboardKeyType keyTypesFilter
            {
                set
                {
#if UNITY_EDITOR
                    m_Value.keyTypesFilter = (HiraBots.BlackboardKeyType) (byte) value;
#endif
                }
            }

            /// <summary>
            /// Set the filter for the template.
            /// </summary>
            public void OnTargetBlackboardTemplateChanged(BlackboardTemplate newTemplate, in KeySet keySet)
            {
#if UNITY_EDITOR
                m_Value.OnTargetBlackboardTemplateChanged(newTemplate.m_Value, keySet.m_Value);
#endif
            }

            /// <summary>
            /// Validate the key selector.
            /// </summary>
            public bool Validate(in KeySet keySet, BlackboardKeyType keyType)
            {
                var context = new BlackboardTemplateKeySelectorValidatorContext
                {
                    succeeded = true,
                    allowedKeyPool = keySet.m_Value,
                    allowedKeyTypes = (HiraBots.BlackboardKeyType) (byte) keyType
                };

#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
                m_Value.Validate(ref context);
#endif

                return context.succeeded;
            }
        }

        /// <summary>
        /// A set of keys in a blackboard.
        /// </summary>
        public readonly struct KeySet
        {
            internal KeySet(ReadOnlyHashSetAccessor<HiraBots.BlackboardKey> value)
            {
                m_Value = value;
            }

            internal readonly ReadOnlyHashSetAccessor<HiraBots.BlackboardKey> m_Value;
        }
    }
}