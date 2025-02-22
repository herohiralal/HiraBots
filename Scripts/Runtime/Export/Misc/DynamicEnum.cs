namespace UnityEngine
{
    /// <summary>
    /// An enum value which can have its type selected dynamically from a dropdown in the inspector.
    /// The type identification data is not present in a built player.
    /// </summary>
    [System.Serializable]
    public struct DynamicEnum
    {
#if UNITY_EDITOR
        [SerializeField] private string m_TypeIdentifier;
#endif
        [SerializeField] private byte m_Value;

        public static implicit operator byte(DynamicEnum input) => input.m_Value;

        public string typeIdentifier
        {
            set
            {
#if UNITY_EDITOR
                m_TypeIdentifier = value;
#endif
            }
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            return HiraBots.DynamicEnum.Helpers.identifierToType.TryGetValue(m_TypeIdentifier, out var type)
                ? System.Enum.ToObject(type, m_Value).ToString()
                : m_Value.ToString();
        }
#endif
    }
}