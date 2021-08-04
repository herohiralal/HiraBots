﻿namespace UnityEngine
{
    /// <summary>
    /// A reference to a blackboard template.
    /// </summary>
    [System.Serializable]
    public struct BlackboardTemplate
    {
        internal BlackboardTemplate(HiraBots.BlackboardTemplate value)
        {
            m_Value = value;
        }
        
        // the actual template
        [SerializeField, HideInInspector] private HiraBots.BlackboardTemplate m_Value;

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
        /// Try to convert a key name into a key identifier.
        /// </summary>
        /// <returns>Whether the operation was successful.</returns>
        public bool TryGetKeyIdentifier(string keyName, out ushort identifier)
        {
            if (m_Value.compiledData.keyNameToMemoryOffset.TryGetValue(keyName, out identifier))
            {
                return true;
            }

            identifier = ushort.MaxValue;
            return false;
        }

        /// <summary>
        /// Get instance-synced Boolean value from the blackboard template using the key name.
        /// </summary>
        public bool GetInstanceSyncedBooleanValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedBooleanValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Boolean value from the blackboard template using the key identifier.
        /// </summary>
        public bool GetInstanceSyncedBooleanValue(ushort keyIdentifier)
        {
            return m_Value.compiledData.GetInstanceSyncedBooleanValue(keyIdentifier);
        }

        /// <summary>
        /// Get instance-synced Enum value from the blackboard template using the key name.
        /// </summary>
        public T GetInstanceSyncedEnumValue<T>(string keyName) where T : unmanaged, System.Enum
        {
            return m_Value.compiledData.GetInstanceSyncedEnumValue<T>(keyName);
        }

        /// <summary>
        /// Get instance-synced Enum value from the blackboard template using the key identifier.
        /// </summary>
        public T GetInstanceSyncedEnumValue<T>(ushort keyIdentifier) where T : unmanaged, System.Enum
        {
            return m_Value.compiledData.GetInstanceSyncedEnumValue<T>(keyIdentifier);
        }

        /// <summary>
        /// Get instance-synced Float value from the blackboard template using the key name.
        /// </summary>
        public float GetInstanceSyncedFloatValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedFloatValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Float value from the blackboard template using the key identifier.
        /// </summary>
        public float GetInstanceSyncedFloatValue(ushort keyIdentifier)
        {
            return m_Value.compiledData.GetInstanceSyncedFloatValue(keyIdentifier);
        }

        /// <summary>
        /// Get instance-synced Integer value from the blackboard template using the key name.
        /// </summary>
        public int GetInstanceSyncedIntegerValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedIntegerValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Integer value from the blackboard template using the key identifier.
        /// </summary>
        public int GetInstanceSyncedIntegerValue(ushort keyIdentifier)
        {
            return m_Value.compiledData.GetInstanceSyncedIntegerValue(keyIdentifier);
        }

        /// <summary>
        /// Get instance-synced Object value from the blackboard template using the key name.
        /// </summary>
        public Object GetInstanceSyncedObjectValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedObjectValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Object value from the blackboard template using the key identifier.
        /// </summary>
        public Object GetInstanceSyncedObjectValue(ushort keyIdentifier)
        {
            return m_Value.compiledData.GetInstanceSyncedObjectValue(keyIdentifier);
        }

        /// <summary>
        /// Get instance-synced Vector value from the blackboard template using the key name.
        /// </summary>
        public Vector3 GetInstanceSyncedVectorValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedVectorValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Vector value from the blackboard template using the key identifier.
        /// </summary>
        public Vector3 GetInstanceSyncedVectorValue(ushort keyIdentifier)
        {
            return m_Value.compiledData.GetInstanceSyncedVectorValue(keyIdentifier);
        }

        /// <summary>
        /// Get instance-synced Quaternion value from the blackboard template using the key name.
        /// </summary>
        public Quaternion GetInstanceSyncedQuaternionValue(string keyName)
        {
            return m_Value.compiledData.GetInstanceSyncedQuaternionValue(keyName);
        }

        /// <summary>
        /// Get instance-synced Quaternion value from the blackboard template using the key identifier.
        /// </summary>
        public Quaternion GetInstanceSyncedQuaternionValue(ushort keyIdentifier)
        {
            return m_Value.compiledData.GetInstanceSyncedQuaternionValue(keyIdentifier);
        }

        /// <summary>
        /// Set instance-synced Boolean value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedBooleanValue(string key, bool value)
        {
            m_Value.compiledData.SetInstanceSyncedBooleanValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Boolean value on the blackboard template using the key identifier.
        /// </summary>
        internal void SetInstanceSyncedBooleanValue(ushort keyIdentifier, bool value)
        {
            m_Value.compiledData.SetInstanceSyncedBooleanValue(keyIdentifier, value);
        }

        /// <summary>
        /// Set instance-synced Enum value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedEnumValue<T>(string key, T value) where T : unmanaged, System.Enum
        {
            m_Value.compiledData.SetInstanceSyncedEnumValue<T>(key, value);
        }

        /// <summary>
        /// Set instance-synced Enum value on the blackboard template using the key identifier.
        /// </summary>
        internal void SetInstanceSyncedEnumValue<T>(ushort keyIdentifier, T value) where T : unmanaged, System.Enum
        {
            m_Value.compiledData.SetInstanceSyncedEnumValue<T>(keyIdentifier, value);
        }

        /// <summary>
        /// Set instance-synced Float value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedFloatValue(string key, float value)
        {
            m_Value.compiledData.SetInstanceSyncedFloatValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Float value on the blackboard template using the key identifier.
        /// </summary>
        internal void SetInstanceSyncedFloatValue(ushort keyIdentifier, float value)
        {
            m_Value.compiledData.SetInstanceSyncedFloatValue(keyIdentifier, value);
        }

        /// <summary>
        /// Set instance-synced Integer value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedIntegerValue(string key, int value)
        {
            m_Value.compiledData.SetInstanceSyncedIntegerValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Integer value on the blackboard template using the key identifier.
        /// </summary>
        internal void SetInstanceSyncedIntegerValue(ushort keyIdentifier, int value)
        {
            m_Value.compiledData.SetInstanceSyncedIntegerValue(keyIdentifier, value);
        }

        /// <summary>
        /// Set instance-synced Object value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedObjectValue(string key, Object value)
        {
            m_Value.compiledData.SetInstanceSyncedObjectValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Object value on the blackboard template using the key identifier.
        /// </summary>
        internal void SetInstanceSyncedObjectValue(ushort keyIdentifier, Object value)
        {
            m_Value.compiledData.SetInstanceSyncedObjectValue(keyIdentifier, value);
        }

        /// <summary>
        /// Set instance-synced Vector value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedVectorValue(string key, Vector3 value)
        {
            m_Value.compiledData.SetInstanceSyncedVectorValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Vector value on the blackboard template using the key identifier.
        /// </summary>
        internal void SetInstanceSyncedVectorValue(ushort keyIdentifier, Vector3 value)
        {
            m_Value.compiledData.SetInstanceSyncedVectorValue(keyIdentifier, value);
        }

        /// <summary>
        /// Set instance-synced Quaternion value on the blackboard template using the key name.
        /// </summary>
        internal void SetInstanceSyncedQuaternionValue(string key, Quaternion value)
        {
            m_Value.compiledData.SetInstanceSyncedQuaternionValue(key, value);
        }

        /// <summary>
        /// Set instance-synced Quaternion value on the blackboard template using the key identifier.
        /// </summary>
        internal void SetInstanceSyncedQuaternionValue(ushort keyIdentifier, Quaternion value)
        {
            m_Value.compiledData.SetInstanceSyncedQuaternionValue(keyIdentifier, value);
        }
    }
}