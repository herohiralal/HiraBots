namespace UnityEngine.AI
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
        public bool isValid => m_Value != null;

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
        public string name => m_Value.name;

        /// <summary>
        /// The type of the blackboard key.
        /// </summary>
        public BlackboardKeyType keyType => (BlackboardKeyType) (byte) m_Value.keyType;

        /// <summary>
        /// Whether the key will provide an offset.
        /// </summary>
        public bool canGetOffset => m_Value != null && m_Value.isCompiled;

        /// <summary>
        /// The memory offset of the blackboard key.
        /// </summary>
        public ushort offset => m_Value.compiledData.memoryOffset;

        /// <summary>
        /// Enum type identifier of the key.
        /// </summary>
#if UNITY_EDITOR
        public string enumTypeIdentifier => m_Value is HiraBots.EnumBlackboardKey enumKey ? enumKey.typeIdentifier : "";
#else
        public string enumTypeIdentifier => "";
#endif
    }
}