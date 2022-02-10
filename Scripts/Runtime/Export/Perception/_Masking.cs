namespace UnityEngine.AI
{
    [System.Serializable, System.Flags]
    public enum HiraBotsDefaultStimulusMask : int
    {
        None = 0,
        Sight = 1 << 0,
        Sound = 1 << 1,
    }

    [System.Serializable]
    public struct StimulusType
    {
        [SerializeField] private byte m_Value;

        internal byte ToTypeIndex()
        {
            return m_Value;
        }

        public static implicit operator StimulusType(int value)
        {
            for (byte i = 0; i < 32; i++)
            {
                if ((value >> (i + 1) == 0))
                {
                    return new StimulusType { m_Value = i };
                }
            }

            return new StimulusType { m_Value = 0 };
        }
    }

    [System.Serializable]
    public struct StimulusMask
    {
        public static StimulusMask all => ~0;
        public static StimulusMask none => 0;

        [SerializeField] private int m_Value;

        public static implicit operator int(StimulusMask type)
        {
            return type.m_Value;
        }

        public static implicit operator StimulusMask(int type)
        {
            return new StimulusMask { m_Value = type };
        }
    }
}