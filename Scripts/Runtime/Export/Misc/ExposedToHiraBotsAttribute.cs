using System;

namespace UnityEngine
{
    /// <summary>
    /// Expose a type to HiraBots.
    /// Currently, this is only required for enums, but can probably be used for other things.
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