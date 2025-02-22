namespace UnityEngine.AI
{
    [System.Serializable, System.Flags]
    public enum HiraBotsDefaultStimulusMask : int
    {
        None = 0,
        Sight = 1 << 0,
        Sound = 1 << 1,
        Unused01 = 1 << 2,
        Unused02 = 1 << 3,
        Unused03 = 1 << 4,
        Unused04 = 1 << 5,
        Unused05 = 1 << 6,
        Unused06 = 1 << 7,
        Unused07 = 1 << 8,
        Unused08 = 1 << 9,
        Unused09 = 1 << 10,
        Unused10 = 1 << 11,
        Unused11 = 1 << 12,
        Unused12 = 1 << 13,
        Unused13 = 1 << 14,
        Unused14 = 1 << 15,
        Unused15 = 1 << 16,
        Unused16 = 1 << 17,
        Unused17 = 1 << 18,
        Unused18 = 1 << 19,
        Unused19 = 1 << 20,
        Unused20 = 1 << 21,
        Unused21 = 1 << 22,
        Unused22 = 1 << 23,
        Unused23 = 1 << 24,
        Unused24 = 1 << 25,
        Unused25 = 1 << 26,
        Unused26 = 1 << 27,
        Unused27 = 1 << 28,
        Unused28 = 1 << 29,
        Unused29 = 1 << 30,
    }

    [System.Serializable]
    public struct StimulusType
    {
        [SerializeField] private byte m_Value;

        internal StimulusType(byte value)
        {
            m_Value = value;
        }

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
                    return new StimulusType(i);
                }
            }

            return new StimulusType(0);
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