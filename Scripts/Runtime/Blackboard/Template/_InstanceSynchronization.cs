using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal unsafe interface IInstanceSynchronizerListener
    {
        /// <summary>
        /// Pass on the updated value to the listener.
        /// </summary>
        void UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size);
    }

    internal unsafe partial class BlackboardTemplateCompiledData : IInstanceSynchronizerListener
    {
        // update the value within the template
        void IInstanceSynchronizerListener.UpdateValue(in BlackboardKeyCompiledData keyData, byte* value, ushort size)
        {
            var ptr = templatePtr + keyData.memoryOffset;

            for (var i = 0; i < size; i++)
            {
                ptr[i] = value[i];
            }

            foreach (var listener in m_Listeners)
            {
                // update all the listeners
                listener.UpdateValue(in keyData, value, size);
            }
        }
    }

    internal partial class BlackboardTemplateCompiledData
    {
        private readonly List<IInstanceSynchronizerListener> m_Listeners = new List<IInstanceSynchronizerListener>();

        /// <summary>
        /// Get owning template for a key, based on its memory offset.
        /// </summary>
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

        /// <summary>
        /// Add a listener to the instance synchronization messages.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (!m_Listeners.Contains(listener)) m_Listeners.Add(listener);
        }

        /// <summary>
        /// Remove a listener from the instance synchronization messages.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveInstanceSyncListener(IInstanceSynchronizerListener listener)
        {
            if (m_Listeners.Contains(listener)) m_Listeners.Remove(listener);
        }
    }
}