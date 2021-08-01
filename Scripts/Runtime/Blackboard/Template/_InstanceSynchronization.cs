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
            var ptr = TemplatePtr + keyData.MemoryOffset;
            for (var i = 0; i < size; i++) ptr[i] = value[i];

            foreach (var listener in _listeners)
                listener.UpdateValue(in keyData, value, size);
        }
    }

    internal partial class BlackboardTemplateCompiledData
    {
        private readonly List<IInstanceSynchronizerListener> _listeners = new List<IInstanceSynchronizerListener>();

        internal BlackboardTemplateCompiledData GetOwningTemplate(ushort memoryOffset)
        {
            var current = this;
            var previous = (BlackboardTemplateCompiledData) null;

            do
            {
                if (current.TemplateSize <= memoryOffset) return previous;

                previous = current;
                current = current._parentCompiledData;
            } while (current != null);

            return previous;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (!_listeners.Contains(listener)) _listeners.Add(listener);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (_listeners.Contains(listener)) _listeners.Remove(listener);
        }
    }
}