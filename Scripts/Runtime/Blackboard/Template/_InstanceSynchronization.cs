using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal unsafe interface IInstanceSynchronizerListener
    {
        void UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size);
    }

    internal unsafe partial class BlackboardTemplateCompiledData : IInstanceSynchronizerListener
    {
        void IInstanceSynchronizerListener.UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size)
        {
            var ptr = templatePtr + keyData.m_MemoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            foreach (var listener in m_Listeners)
                listener.UpdateValue(in keyData, value, size);
        }
    }

    internal partial class BlackboardTemplateCompiledData
    {
        private readonly List<IInstanceSynchronizerListener> m_Listeners = new List<IInstanceSynchronizerListener>();

        internal BlackboardTemplateCompiledData GetOwningTemplate(ushort memoryOffset)
        {
            var current = this;
            var previous = (BlackboardTemplateCompiledData) null;

            do
            {
                if (current.templateSize <= memoryOffset) return previous;

                previous = current;
                current = current.m_ParentCompiledData;
            } while (current != null);

            return previous;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (!m_Listeners.Contains(listener)) m_Listeners.Add(listener);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (m_Listeners.Contains(listener)) m_Listeners.Remove(listener);
        }
    }
}