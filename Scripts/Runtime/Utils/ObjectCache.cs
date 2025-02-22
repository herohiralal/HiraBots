using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    internal struct ObjectCache
    {
        internal ObjectCache(int startingCap)
        {
            m_Value = new Dictionary<int, (Object obj, int count)>(startingCap);
        }

        private readonly Dictionary<int, (Object obj, int count)> m_Value;

        internal void Clear()
        {
            m_Value.Clear();
        }

        internal void Add(int id, Object obj)
        {
            if (id == 0)
            {
                return;
            }

            if (m_Value.TryGetValue(id, out var val))
            {
                val.count++;
                m_Value[id] = val;
            }
            else
            {
                m_Value.Add(id, (obj, 1));
            }
        }

        internal void Remove(int id)
        {
            if (!m_Value.TryGetValue(id, out var val))
            {
                return;
            }

            val.count--;
            if (val.count > 0)
            {
                m_Value[id] = val;
            }
            else
            {
                m_Value.Remove(id);
            }
        }

        internal bool TryGetValue(int id, out Object o)
        {
            var res = m_Value.TryGetValue(id, out var ret);
            o = ret.obj;
            return res;
        }

        internal IEnumerable<Object> GetAllObjects()
        {
            foreach (var (o, _) in m_Value.Values)
            {
                yield return o;
            }
        }
    }
}