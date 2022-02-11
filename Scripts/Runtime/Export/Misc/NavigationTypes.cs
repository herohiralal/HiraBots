namespace UnityEngine.AI
{
    [System.Serializable]
    public struct NavAgentType
    {
        [SerializeField] private int m_Value;

        public static implicit operator int(NavAgentType x)
        {
            return x.m_Value;
        }

        public static implicit operator NavAgentType(int value)
        {
            return new NavAgentType { m_Value = value };
        }
    }

    [System.Serializable]
    public struct NavAreaMask
    {
        [SerializeField] private int m_Value;

        public static implicit operator int(NavAreaMask x)
        {
            return x.m_Value;
        }

        public static implicit operator NavAreaMask(int value)
        {
            return new NavAreaMask { m_Value = value };
        }
    }
}