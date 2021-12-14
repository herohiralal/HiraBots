using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots
{
    internal static class CompilationRegistry
    {
        internal readonly struct Entry
        {
            internal Entry(string name, IntPtr startAddress, IntPtr endAddress)
            {
                this.name = name;
                this.startAddress = startAddress;
                this.endAddress = endAddress;
            }

            internal string name { get; }
            internal IntPtr startAddress { get; }
            internal IntPtr endAddress { get; }

            internal ushort size => (ushort) (1L + (long) endAddress - (long) startAddress);
        }

        private class Builder
        {
            internal Dictionary<string, List<List<Entry>>> m_Data;
            internal string m_CurrentObject;
            internal ushort m_CurrentDepth;
        }

        static CompilationRegistry()
        {
            database = new Dictionary<string, ReadOnlyArrayAccessor<ReadOnlyArrayAccessor<Entry>>>().ReadOnly();
        }

        private static Builder s_Builder;
        internal static ReadOnlyDictionaryAccessor<string, ReadOnlyArrayAccessor<ReadOnlyArrayAccessor<Entry>>> database { get; private set; }

        [Conditional("UNITY_EDITOR")]
        internal static void Initialize()
        {
            s_Builder = new Builder
            {
                m_Data = new Dictionary<string, List<List<Entry>>>(),
                m_CurrentObject = null,
                m_CurrentDepth = 0
            };
        }

        [Conditional("UNITY_EDITOR")]
        internal static void BeginObject(Object o)
        {
            var name = $"{o.name} [{o.GetInstanceID()}]";
            s_Builder.m_Data.Add(name, new List<List<Entry>>(0));
            s_Builder.m_Data[name].Add(new List<Entry>(0));
            s_Builder.m_CurrentObject = name;
            s_Builder.m_CurrentDepth = 0;
        }

        [Conditional("UNITY_EDITOR")]
        internal static void IncreaseDepth()
        {
            s_Builder.m_CurrentDepth++;
            var l = s_Builder.m_Data[s_Builder.m_CurrentObject];
            if (l.Count == s_Builder.m_CurrentDepth)
            {
                l.Add(new List<Entry>(0));
            }
        }

        [Conditional("UNITY_EDITOR")]
        internal static unsafe void AddEntry(string name, byte* startAddress, byte* endAddress)
        {
            s_Builder.m_Data[s_Builder.m_CurrentObject][s_Builder.m_CurrentDepth].Add(new Entry(name, (IntPtr) startAddress, (IntPtr) (endAddress - 1)));
        }

        internal static void DecreaseDepth()
        {
            s_Builder.m_CurrentDepth--;
        }

        [Conditional("UNITY_EDITOR")]
        internal static void EndObject()
        {
            s_Builder.m_CurrentDepth = 0;
            s_Builder.m_CurrentObject = null;
        }

        [Conditional("UNITY_EDITOR")]
        internal static void Build()
        {
            var d = new Dictionary<string, ReadOnlyArrayAccessor<ReadOnlyArrayAccessor<Entry>>>();

            foreach (var kvp in s_Builder.m_Data)
            {
                var currentObject = kvp.Key;
                var layersInCurrentObject = kvp.Value;

                var layers = new ReadOnlyArrayAccessor<Entry>[layersInCurrentObject.Count];
                for (var currentDepth = 0; currentDepth < layersInCurrentObject.Count; currentDepth++)
                {
                    var entriesAtCurrentDepth = layersInCurrentObject[currentDepth];
                    layers[currentDepth] = entriesAtCurrentDepth.ToArray().ReadOnly();
                }

                d.Add(currentObject, layers.ReadOnly());
            }

            database = d.ReadOnly();

            s_Builder = null;
        }
    }
}