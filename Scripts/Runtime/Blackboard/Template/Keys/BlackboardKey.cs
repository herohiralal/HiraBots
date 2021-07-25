using System;
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardKey : ScriptableObject
    {
        protected internal BlackboardKey(byte sizeInBytes, BlackboardKeyType keyType) =>
            (SizeInBytes, KeyType) = (sizeInBytes, keyType);

        [SerializeField, HideInInspector] private bool instanceSynced = false;
        [SerializeField, HideInInspector] private bool essentialToDecisionMaking = false;

        [NonSerialized] internal readonly byte SizeInBytes;
        [NonSerialized] internal readonly BlackboardKeyType KeyType;

#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, HideInInspector] private bool isExpanded = false;
#pragma warning restore CS0414
#endif
    }
}