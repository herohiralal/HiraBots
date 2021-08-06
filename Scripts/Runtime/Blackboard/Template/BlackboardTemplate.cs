using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// A blackboard template to make blackboard components out of.
    /// </summary>
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "HiraBots/Blackboard")]
    internal partial class BlackboardTemplate : ScriptableObject
    {
        [Tooltip("The parent template of this one. A blackboard template inherits all of its parents' keys.")]
        [SerializeField, HideInInspector] private BlackboardTemplate m_Parent = null;

        [Tooltip("The keys within this blackboard template.")]
        [SerializeField, HideInInspector] private BlackboardKey[] m_Keys = new BlackboardKey[0];

        /// <summary>
        /// Get a set of keys present in the blackboard. Optionally, include/exclude inherited keys.
        /// </summary>
        internal void GetKeySet(System.Collections.Generic.HashSet<BlackboardKey> keys, bool includeInherited = true)
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
    }
}