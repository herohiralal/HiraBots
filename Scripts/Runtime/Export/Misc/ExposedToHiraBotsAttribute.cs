using System;

namespace UnityEngine.AI
{
    /// <summary>
    /// Expose a type to HiraBots.
    /// Usage: add to an 8-bit enum to expose it as a blackboard key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class ExposedToHiraBotsAttribute : Attribute
    {
        public ExposedToHiraBotsAttribute(string identifier)
        {
            this.identifier = identifier;
        }

        internal string identifier { get; }
    }
}