namespace UnityEngine.AI
{
    [System.Flags]
    public enum HiraBotsDefaultStimulusMask : int
    {
        None = 0,
        Sight = 1 << 0,
        Sound = 1 << 1,
    }
}