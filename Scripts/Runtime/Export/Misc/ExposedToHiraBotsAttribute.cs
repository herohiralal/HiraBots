using System;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class ExposedToHiraBotsAttribute : Attribute
    {
        public ExposedToHiraBotsAttribute(string identifier) => m_Identifier = identifier;
        internal readonly string m_Identifier;
    }
}