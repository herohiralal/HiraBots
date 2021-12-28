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
        internal HiraBots.BlackboardComponent m_Value;

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
        /// Get Enum value index from the blackboard component using the key name.
        /// </summary>
        public byte GetEnumValue(string keyName)
        {
            return m_Value.GetEnumValue(keyName);
        }

        /// <summary>
        /// Get Enum value from the blackboard component using the key name.
        /// </summary>
        public T GetEnumValue<T>(string keyName) where T : unmanaged, System.Enum
        {
            return m_Value.GetEnumValue<T>(keyName);
        }

        /// <summary>
        /// Get Float value from the blackboard component using the key name.
        /// </summary>
        public float GetFloatValue(string keyName)
        {
            return m_Value.GetFloatValue(keyName);
        }

        /// <summary>
        /// Get Integer value from the blackboard component using the key name.
        /// </summary>
        public int GetIntegerValue(string keyName)
        {
            return m_Value.GetIntegerValue(keyName);
        }

        /// <summary>
        /// Get Object value from the blackboard component using the key name.
        /// </summary>
        public Object GetObjectValue(string keyName)
        {
            return m_Value.GetObjectValue(keyName);
        }

        /// <summary>
        /// Get Vector value from the blackboard component using the key name.
        /// </summary>
        public Vector3 GetVectorValue(string keyName)
        {
            return m_Value.GetVectorValue(keyName);
        }

        /// <summary>
        /// Get Quaternion value from the blackboard component using the key name.
        /// </summary>
        public Quaternion GetQuaternionValue(string keyName)
        {
            return m_Value.GetQuaternionValue(keyName);
        }

        /// <summary>
        /// Set Boolean value on the blackboard component using the key name.
        /// </summary>
        public void SetBooleanValue(string key, bool value, bool expected = false)
        {
            m_Value.SetBooleanValue(key, value, expected);
        }

        /// <summary>
        /// Set Enum value index on the blackboard component using the key name.
        /// </summary>
        public void SetEnumValue(string key, byte value, bool expected = false)
        {
            m_Value.SetEnumValue(key, value, expected);
        }

        /// <summary>
        /// Set Enum value on the blackboard component using the key name.
        /// </summary>
        public void SetEnumValue<T>(string key, T value, bool expected = false) where T : unmanaged, System.Enum
        {
            m_Value.SetEnumValue<T>(key, value, expected);
        }

        /// <summary>
        /// Set Float value on the blackboard component using the key name.
        /// </summary>
        public void SetFloatValue(string key, float value, bool expected = false)
        {
            m_Value.SetFloatValue(key, value, expected);
        }

        /// <summary>
        /// Set Integer value on the blackboard component using the key name.
        /// </summary>
        public void SetIntegerValue(string key, int value, bool expected = false)
        {
            m_Value.SetIntegerValue(key, value, expected);
        }

        /// <summary>
        /// Set Object value on the blackboard component using the key name.
        /// </summary>
        public void SetObjectValue(string key, Object value, bool expected = false)
        {
            m_Value.SetObjectValue(key, value, expected);
        }

        /// <summary>
        /// Set Vector value on the blackboard component using the key name.
        /// </summary>
        public void SetVectorValue(string key, Vector3 value, bool expected = false)
        {
            m_Value.SetVectorValue(key, value, expected);
        }

        /// <summary>
        /// Set Quaternion value on the blackboard component using the key name.
        /// </summary>
        public void SetQuaternionValue(string key, Quaternion value, bool expected = false)
        {
            m_Value.SetQuaternionValue(key, value, expected);
        }
    }
}