using System;
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardKey : ScriptableObject
    {
        protected internal BlackboardKey()
        {
        }

        [SerializeField, HideInInspector] protected bool m_InstanceSynced = false;
        [SerializeField, HideInInspector] protected bool m_EssentialToDecisionMaking = false;

        [NonSerialized] protected byte m_SizeInBytesInternal;
        internal byte sizeInBytes => m_SizeInBytesInternal;

        [NonSerialized] protected BlackboardKeyType m_KeyType;
    }
}