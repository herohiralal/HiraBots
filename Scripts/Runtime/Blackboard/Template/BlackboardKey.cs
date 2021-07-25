using System;
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardKey : ScriptableObject
    {
        protected internal BlackboardKey()
        {
        }

        [SerializeField, HideInInspector] protected bool instanceSynced = false;
        [SerializeField, HideInInspector] protected bool essentialToDecisionMaking = false;

        [NonSerialized] protected byte SizeInBytesInternal;
        internal byte SizeInBytes => SizeInBytesInternal;

        [NonSerialized] protected BlackboardKeyType KeyType;
    }
}