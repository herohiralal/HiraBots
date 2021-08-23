using System;
using System.Collections.Generic;

namespace UnityEngine
{
    // ReSharper disable once InconsistentNaming
    public abstract class Base_GeneratedBlackboardComponent : IDisposable
    {
        private static readonly HashSet<Base_GeneratedBlackboardComponent> s_ActiveComponents =
            new HashSet<Base_GeneratedBlackboardComponent>();

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