using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// A blackboard template to make blackboard components out of.
    /// </summary>
    internal partial class BlackboardTemplate : ScriptableObject
    {
        [Tooltip("The backends to use.")]
        [SerializeField, HideInInspector] private BackendType m_Backends = BackendType.RuntimeInterpreter;

        [Tooltip("The parent template of this one. A blackboard template inherits all of its parents' keys.")]
        [SerializeField, HideInInspector] private BlackboardTemplate m_Parent = null;

        [Tooltip("The keys within this blackboard template.")]
        [SerializeField, HideInInspector] private BlackboardKey[] m_Keys = new BlackboardKey[0];

        /// <summary>
        /// The effective backend to use.
        /// </summary>
        internal BackendType backends => m_Backends;

        /// <summary>
        /// Get a set of keys present in the blackboard. Optionally, include/exclude inherited keys.
        /// </summary>
        internal void GetKeySet(HashSet<BlackboardKey> keys, bool includeInherited = true)
        {
            if (includeInherited && m_Parent != null)
            {
                // inherited
                m_Parent.GetKeySet(keys);
            }

            // own
            foreach (var key in m_Keys)
            {
                keys.Add(key);
            }
        }

        /// <summary>
        /// Implicitly convert a BlackboardTemplate to its public interface.
        /// </summary>
        public static implicit operator UnityEngine.BlackboardTemplate(BlackboardTemplate actualTemplate)
        {
            return new UnityEngine.BlackboardTemplate(actualTemplate);
        }

        /// <summary>
        /// Get keys sorted by their size in bytes.
        /// </summary>
        private IEnumerable<BlackboardKey> sortedKeysExcludingInherited =>
            m_Keys.OrderBy(k => k.sizeInBytes);

        /// <summary>
        /// Get keys (including inherited) sorted by their size in bytes.
        /// </summary>
        private IEnumerable<BlackboardKey> sortedKeysIncludingInherited
        {
            get
            {
                var empty = Enumerable.Empty<BlackboardKey>();

                if (m_Parent != null)
                {
                    empty = empty.Concat(m_Parent.sortedKeysIncludingInherited);
                }

                empty = empty.Concat(sortedKeysExcludingInherited);
                return empty;
            }
        }
    }
}