﻿using UnityEngine;

namespace HiraBots
{
    internal partial class VectorBlackboardKey : BlackboardKey
    {
        internal VectorBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(float) * 3;
            m_KeyType = BlackboardKeyType.Vector;
        }

        [SerializeField] private Vector3 defaultValue = Vector3.zero;
    }
}