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
        [NonSerialized] protected byte m_SizeInBytesInternal;
        internal byte sizeInBytes => m_SizeInBytesInternal;

        /// <summary>
        /// The type of the value this key would be.
        /// </summary>
        [NonSerialized] protected BlackboardKeyType m_KeyType;
    }
}