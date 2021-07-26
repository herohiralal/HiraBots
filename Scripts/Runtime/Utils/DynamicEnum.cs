using System;
using UnityEngine;

namespace HiraBots
{
    [Serializable]
    internal struct DynamicEnum
    {
#if UNITY_EDITOR
        [SerializeField] internal string typeIdentifier;
#endif
        [SerializeField] internal byte value;

        public static implicit operator byte(DynamicEnum input) => input.value;
    }
}