using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class ExposedToHiraBotsAttribute : Attribute
    {
        public ExposedToHiraBotsAttribute(string identifier) => Identifier = identifier;
        internal readonly string Identifier;
    }
}