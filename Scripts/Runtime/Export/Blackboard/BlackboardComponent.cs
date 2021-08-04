namespace UnityEngine
{
    /// <summary>
    /// A reference to a blackboard component.
    /// </summary>
    [System.Serializable]
    public struct BlackboardComponent
    {
        internal BlackboardComponent(HiraBots.BlackboardComponent value)
        {
            m_Value = value;
        }

        // the actual component
        private HiraBots.BlackboardComponent m_Value;

        /// <summary>
        /// Whether the blackboard component is valid.
        /// </summary>
        public bool isValid => m_Value != null;

        /// <summary>
        /// Implicit boolean cast to determine validity.
        /// </summary>
        public static implicit operator bool(BlackboardComponent component)
        {
            return component.isValid;
        }

        /// <summary>
        /// Get Boolean value from the blackboard component using the key name.
        /// </summary>
        public bool GetBooleanValue(string keyName)
        {
            return m_Value.GetBooleanValue(keyName);
        }

        /// <summary>
        /// Get Boolean value from the blackboard component using the key identifier.
        /// </summary>
        public bool GetBooleanValue(ushort keyIdentifier)
        {
            return m_Value.GetBooleanValue(keyIdentifier);
        }

        /// <summary>
        /// Get Enum value from the blackboard component using the key name.
        /// </summary>
        public T GetEnumValue<T>(string keyName) where T : unmanaged, System.Enum
        {
            return m_Value.GetEnumValue<T>(keyName);
        }

        /// <summary>
        /// Get Enum value from the blackboard component using the key identifier.
        /// </summary>
        public T GetEnumValue<T>(ushort keyIdentifier) where T : unmanaged, System.Enum
        {
            return m_Value.GetEnumValue<T>(keyIdentifier);
        }

        /// <summary>
        /// Get Float value from the blackboard component using the key name.
        /// </summary>
        public float GetFloatValue(string keyName)
        {
            return m_Value.GetFloatValue(keyName);
        }

        /// <summary>
        /// Get Float value from the blackboard component using the key identifier.
        /// </summary>
        public float GetFloatValue(ushort keyIdentifier)
        {
            return m_Value.GetFloatValue(keyIdentifier);
        }

        /// <summary>
        /// Get Integer value from the blackboard component using the key name.
        /// </summary>
        public int GetIntegerValue(string keyName)
        {
            return m_Value.GetIntegerValue(keyName);
        }

        /// <summary>
        /// Get Integer value from the blackboard component using the key identifier.
        /// </summary>
        public int GetIntegerValue(ushort keyIdentifier)
        {
            return m_Value.GetIntegerValue(keyIdentifier);
        }

        /// <summary>
        /// Get Object value from the blackboard component using the key name.
        /// </summary>
        public Object GetObjectValue(string keyName)
        {
            return m_Value.GetObjectValue(keyName);
        }

        /// <summary>
        /// Get Object value from the blackboard component using the key identifier.
        /// </summary>
        public Object GetObjectValue(ushort keyIdentifier)
        {
            return m_Value.GetObjectValue(keyIdentifier);
        }

        /// <summary>
        /// Get Vector value from the blackboard component using the key name.
        /// </summary>
        public Vector3 GetVectorValue(string keyName)
        {
            return m_Value.GetVectorValue(keyName);
        }

        /// <summary>
        /// Get Vector value from the blackboard component using the key identifier.
        /// </summary>
        public Vector3 GetVectorValue(ushort keyIdentifier)
        {
            return m_Value.GetVectorValue(keyIdentifier);
        }

        /// <summary>
        /// Get Quaternion value from the blackboard component using the key name.
        /// </summary>
        public Quaternion GetQuaternionValue(string keyName)
        {
            return m_Value.GetQuaternionValue(keyName);
        }

        /// <summary>
        /// Get Quaternion value from the blackboard component using the key identifier.
        /// </summary>
        public Quaternion GetQuaternionValue(ushort keyIdentifier)
        {
            return m_Value.GetQuaternionValue(keyIdentifier);
        }

        /// <summary>
        /// Set Boolean value on the blackboard component using the key name.
        /// </summary>
        internal void SetBooleanValue(string key, bool value, bool expected = false)
        {
            m_Value.SetBooleanValue(key, value, expected);
        }

        /// <summary>
        /// Set Boolean value on the blackboard component using the key identifier.
        /// </summary>
        internal void SetBooleanValue(ushort keyIdentifier, bool value, bool expected = false)
        {
            m_Value.SetBooleanValue(keyIdentifier, value, expected);
        }

        /// <summary>
        /// Set Enum value on the blackboard component using the key name.
        /// </summary>
        internal void SetEnumValue<T>(string key, T value, bool expected = false) where T : unmanaged, System.Enum
        {
            m_Value.SetEnumValue<T>(key, value, expected);
        }

        /// <summary>
        /// Set Enum value on the blackboard component using the key identifier.
        /// </summary>
        internal void SetEnumValue<T>(ushort keyIdentifier, T value, bool expected = false) where T : unmanaged, System.Enum
        {
            m_Value.SetEnumValue<T>(keyIdentifier, value, expected);
        }

        /// <summary>
        /// Set Float value on the blackboard component using the key name.
        /// </summary>
        internal void SetFloatValue(string key, float value, bool expected = false)
        {
            m_Value.SetFloatValue(key, value, expected);
        }

        /// <summary>
        /// Set Float value on the blackboard component using the key identifier.
        /// </summary>
        internal void SetFloatValue(ushort keyIdentifier, float value, bool expected = false)
        {
            m_Value.SetFloatValue(keyIdentifier, value, expected);
        }

        /// <summary>
        /// Set Integer value on the blackboard component using the key name.
        /// </summary>
        internal void SetIntegerValue(string key, int value, bool expected = false)
        {
            m_Value.SetIntegerValue(key, value, expected);
        }

        /// <summary>
        /// Set Integer value on the blackboard component using the key identifier.
        /// </summary>
        internal void SetIntegerValue(ushort keyIdentifier, int value, bool expected = false)
        {
            m_Value.SetIntegerValue(keyIdentifier, value, expected);
        }

        /// <summary>
        /// Set Object value on the blackboard component using the key name.
        /// </summary>
        internal void SetObjectValue(string key, Object value, bool expected = false)
        {
            m_Value.SetObjectValue(key, value, expected);
        }

        /// <summary>
        /// Set Object value on the blackboard component using the key identifier.
        /// </summary>
        internal void SetObjectValue(ushort keyIdentifier, Object value, bool expected = false)
        {
            m_Value.SetObjectValue(keyIdentifier, value, expected);
        }

        /// <summary>
        /// Set Vector value on the blackboard component using the key name.
        /// </summary>
        internal void SetVectorValue(string key, Vector3 value, bool expected = false)
        {
            m_Value.SetVectorValue(key, value, expected);
        }

        /// <summary>
        /// Set Vector value on the blackboard component using the key identifier.
        /// </summary>
        internal void SetVectorValue(ushort keyIdentifier, Vector3 value, bool expected = false)
        {
            m_Value.SetVectorValue(keyIdentifier, value, expected);
        }

        /// <summary>
        /// Set Quaternion value on the blackboard component using the key name.
        /// </summary>
        internal void SetQuaternionValue(string key, Quaternion value, bool expected = false)
        {
            m_Value.SetQuaternionValue(key, value, expected);
        }

        /// <summary>
        /// Set Quaternion value on the blackboard component using the key identifier.
        /// </summary>
        internal void SetQuaternionValue(ushort keyIdentifier, Quaternion value, bool expected = false)
        {
            m_Value.SetQuaternionValue(keyIdentifier, value, expected);
        }
    }
}