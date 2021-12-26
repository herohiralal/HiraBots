namespace UnityEngine
{
    [System.Serializable]
    public struct BlackboardKey
    {
        internal BlackboardKey(HiraBots.BlackboardKey value)
        {
            m_Value = value;
        }
        
        // the actual template
        [SerializeField, HideInInspector] internal HiraBots.BlackboardKey m_Value;

        /// <summary>
        /// Whether the blackboard template is valid.
        /// </summary>
        public bool isValid => m_Value != null && m_Value.isCompiled;

        /// <summary>
        /// Implicit boolean cast to determine validity.
        /// </summary>
        public static implicit operator bool(BlackboardKey template)
        {
            return template.isValid;
        }

        /// <summary>
        /// The name of the blackboard key.
        /// </summary>
        public string name => m_Value.compiledData.keyName;

        /// <summary>
        /// The type of the blackboard key.
        /// </summary>
        public BlackboardKeyType keyType => (BlackboardKeyType) (byte) m_Value.compiledData.keyType;

        /// <summary>
        /// Whether the key is supposed to have a synchronized instance.
        /// </summary>
        public bool instanceSynced => m_Value.compiledData.instanceSynced;
    }
}