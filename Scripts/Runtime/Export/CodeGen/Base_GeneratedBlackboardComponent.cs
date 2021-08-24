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

        public abstract bool GetBooleanValue(string key);
        public abstract void SetBooleanValue(string key, bool value, bool expected = false);
        public abstract T GetEnumValue<T>(string key) where T : unmanaged, Enum;
        public abstract void SetEnumValue<T>(string key, T value, bool expected = false) where T : unmanaged, Enum;
        public abstract float GetFloatValue(string key);
        public abstract void SetFloatValue(string key, float value, bool expected = false);
        public abstract int GetIntegerValue(string key);
        public abstract void SetIntegerValue(string key, int value, bool expected = false);
        public abstract Object GetObjectValue(string key);
        public abstract void SetObjectValue(string key, Object value, bool expected = false);
        public abstract Quaternion GetQuaternionValue(string key);
        public abstract void SetQuaternionValue(string key, Quaternion value, bool expected = false);
        public abstract Vector3 GetVectorValue(string key);
        public abstract void SetVectorValue(string key, Vector3 value, bool expected = false);
    }
}