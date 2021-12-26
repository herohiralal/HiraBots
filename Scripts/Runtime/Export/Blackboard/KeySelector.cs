using HiraBots;

namespace UnityEngine
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
                m_KeyType = BlackboardKeyType.Any;
            }

            [SerializeField, HideInInspector] internal HiraBots.BlackboardTemplate.KeySelector m_Value;
            [SerializeField, HideInInspector] private BlackboardKeyType m_KeyType;

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
            public void SetKeyTypesFilterToInvalid()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Invalid;
#endif
                m_KeyType = BlackboardKeyType.Invalid;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToBoolean()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Boolean;
#endif
                m_KeyType = BlackboardKeyType.Boolean;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToEnum()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Enum;
#endif
                m_KeyType = BlackboardKeyType.Enum;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToFloat()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Float;
#endif
                m_KeyType = BlackboardKeyType.Float;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToInteger()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Integer;
#endif
                m_KeyType = BlackboardKeyType.Integer;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToObject()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Object;
#endif
                m_KeyType = BlackboardKeyType.Object;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToQuaternion()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Quaternion;
#endif
                m_KeyType = BlackboardKeyType.Quaternion;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToVector()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Vector;
#endif
                m_KeyType = BlackboardKeyType.Vector;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToNumeric()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Numeric;
#endif
                m_KeyType = BlackboardKeyType.Numeric;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToUnmanagedSettable()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.UnmanagedSettable;
#endif
                m_KeyType = BlackboardKeyType.UnmanagedSettable;
            }

            /// <summary>
            /// Set the filter for the types of keys allowed.
            /// </summary>
            public void SetKeyTypesFilterToAny()
            {
#if UNITY_EDITOR
                m_Value.keyTypesFilter = BlackboardKeyType.Any;
#endif
                m_KeyType = BlackboardKeyType.Any;
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
            public bool Validate(in KeySet keySet)
            {
                var context = new BlackboardTemplateKeySelectorValidatorContext
                {
                    succeeded = true,
                    allowedKeyPool = keySet.m_Value,
                    allowedKeyTypes = m_KeyType
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