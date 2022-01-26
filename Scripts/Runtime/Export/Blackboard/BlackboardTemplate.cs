namespace UnityEngine.AI
{
    /// <summary>
    /// A reference to a blackboard template.
    /// </summary>
    [System.Serializable]
    public partial struct BlackboardTemplate
    {
        internal BlackboardTemplate(HiraBots.BlackboardTemplate value)
        {
            m_Value = value;
        }
        
        // the actual template
        [SerializeField, HideInInspector] internal HiraBots.BlackboardTemplate m_Value;

        /// <summary>
        /// Whether the blackboard template is valid.
        /// </summary>
        public bool isValid => m_Value != null && m_Value.isCompiled;

        /// <summary>
        /// Implicit boolean cast to determine validity.
        /// </summary>
        public static implicit operator bool(BlackboardTemplate template)
        {
            return template.isValid;
        }

        /// <summary>
        /// The name of the blackboard template.
        /// </summary>
        public string name => m_Value.name;

        /// <summary>
        /// Get instance-synced Boolean value from the blackboard template using the key name.
        /// </summary>
        public bool GetInstanceSyncedBooleanValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedBooleanValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Enum value index from the blackboard template using the key name.
        /// </summary>
        public byte GetInstanceSyncedEnumValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedEnumValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Enum value from the blackboard template using the key name.
        /// </summary>
        public T GetInstanceSyncedEnumValue<T>(string keyName) where T : unmanaged, System.Enum
        {
            return m_Value.compiledData.GetInstanceSyncedEnumValue<T>(keyName);
        }

        /// <summary>
        /// Get instance-synced Float value from the blackboard template using the key name.
        /// </summary>
        public float GetInstanceSyncedFloatValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedFloatValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Integer value from the blackboard template using the key name.
        /// </summary>
        public int GetInstanceSyncedIntegerValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedIntegerValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Object value from the blackboard template using the key name.
        /// </summary>
        public Object GetInstanceSyncedObjectValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedObjectValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Vector value from the blackboard template using the key name.
        /// </summary>
        public Unity.Mathematics.float3 GetInstanceSyncedVectorValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedVectorValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Quaternion value from the blackboard template using the key name.
        /// </summary>
        public Unity.Mathematics.quaternion GetInstanceSyncedQuaternionValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedQuaternionValue(keyName);
        }

        /// <summary>
        /// Set instance-synced Boolean value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedBooleanValue(string key, bool value)
        {
            m_Value.compiledData.SetInstanceSyncedBooleanValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Enum value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedEnumValue(string key, byte value)
        {
            m_Value.compiledData.SetInstanceSyncedEnumValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Enum value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedEnumValue<T>(string key, T value) where T : unmanaged, System.Enum
        {
            m_Value.compiledData.SetInstanceSyncedEnumValue<T>(key, value);
        }

        /// <summary>
        /// Set instance-synced Float value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedFloatValue(string key, float value)
        {
            m_Value.compiledData.SetInstanceSyncedFloatValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Integer value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedIntegerValue(string key, int value)
        {
            m_Value.compiledData.SetInstanceSyncedIntegerValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Object value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedObjectValue(string key, Object value)
        {
            m_Value.compiledData.SetInstanceSyncedObjectValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Vector value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedVectorValue(string key, Unity.Mathematics.float3 value)
        {
            m_Value.compiledData.SetInstanceSyncedVectorValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Quaternion value on the blackboard template using the key name.
        /// </summary>
        public void SetInstanceSyncedQuaternionValue(string key, Unity.Mathematics.quaternion value)
        {
            m_Value.compiledData.SetInstanceSyncedQuaternionValue(key, value);
        }
    }
}