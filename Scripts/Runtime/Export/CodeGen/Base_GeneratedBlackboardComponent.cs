using System;
using System.Collections.Generic;

namespace UnityEngine
{
    // ReSharper disable once InconsistentNaming
    public abstract class Base_GeneratedBlackboardComponent : IDisposable
    {
        private static readonly HashSet<Base_GeneratedBlackboardComponent> s_ActiveComponents =
            new HashSet<Base_GeneratedBlackboardComponent>();

        protected readonly List<string> m_UnexpectedChanges = new List<string>();

        public ReadOnlyListAccessor<string> unexpectedChanges => m_UnexpectedChanges;

        public bool hasUnexpectedChanges => m_UnexpectedChanges.Count > 0;

        public void ClearUnexpectedChanges()
        {
            m_UnexpectedChanges.Clear();
        }

        protected static void Register(Base_GeneratedBlackboardComponent component)
        {
            s_ActiveComponents.Add(component);
        }

        protected static void Unregister(Base_GeneratedBlackboardComponent component)
        {
            s_ActiveComponents.Remove(component);
        }

        public abstract void Dispose();
    }
}