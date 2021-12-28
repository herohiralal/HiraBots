namespace UnityEngine
{
    /// <summary>
    /// A reference to an LGOAP domain.
    /// </summary>
    [System.Serializable]
    public struct LGOAPDomain
    {
        internal LGOAPDomain(HiraBots.LGOAPDomain value)
        {
            m_Value = value;
        }

        // the actual template
        [SerializeField, HideInInspector] internal HiraBots.LGOAPDomain m_Value;

        /// <summary>
        /// Whether the blackboard template is valid.
        /// </summary>
        public bool isValid => m_Value != null && m_Value.isCompiled;

        /// <summary>
        /// Implicit boolean cast to determine validity.
        /// </summary>
        public static implicit operator bool(LGOAPDomain template)
        {
            return template.isValid;
        }

        /// <summary>
        /// The name of the blackboard template.
        /// </summary>
        public string name => m_Value.name;

        /// <summary>
        /// The blackboard to use for this domain.
        /// </summary>
        public BlackboardTemplate blackboard => m_Value.blackboard;
    }
}