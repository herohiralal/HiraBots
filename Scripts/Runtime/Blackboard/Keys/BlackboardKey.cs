using System;
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardKey : ScriptableObject
    {
        [Tooltip("Synchronize the value corresponding to this key between all instances.")]
        [SerializeField, HideInInspector] protected bool m_InstanceSynced = false;
        
        [Tooltip("Whether this key is essential to decision-making. Any unexpected changes to it will trigger AI algorithms to run again.")]
        [SerializeField, HideInInspector] protected bool m_EssentialToDecisionMaking = false;

        /// <summary>
        /// The size a value of this key would take.
        /// </summary>
        internal byte sizeInBytes => m_SizeInBytes;
        [NonSerialized] protected byte m_SizeInBytes;

        /// <summary>
        /// The type of the value this key would be.
        /// </summary>
        internal BlackboardKeyType keyType => m_KeyType;
        [NonSerialized] protected BlackboardKeyType m_KeyType;

        /// <summary>
        /// Implicitly convert a KeySelector to its public interface.
        /// </summary>
        public static implicit operator UnityEngine.BlackboardKey(BlackboardKey key)
        {
            return new UnityEngine.BlackboardKey(key);
        }
    }
}