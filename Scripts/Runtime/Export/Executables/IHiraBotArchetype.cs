namespace UnityEngine.AI
{
    /// <summary>
    /// An archetype contains components in a way to avoid GetComponent calls.
    /// </summary>
    public interface IHiraBotArchetype
    {
        /// <summary>
        /// The primary GameObject of the archetype.
        /// </summary>
        GameObject gameObject { get; }
    }

    /// <summary>
    /// An archetype contains components in a way to avoid GetComponent calls.
    /// </summary>
    public interface IHiraBotArchetype<out T> : IHiraBotArchetype
        where T : Component
    {
        /// <summary>
        /// The respective component.
        /// </summary>
        T component { get; }
    }
}