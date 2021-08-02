﻿using UnityEngine;

namespace HiraBots
{
    internal partial class ObjectBlackboardKey : BlackboardKey
    {
        internal ObjectBlackboardKey()
        {
            m_SizeInBytesInternal = sizeof(int);
            m_KeyType = BlackboardKeyType.Object;
        }

        [SerializeField] private Object defaultValue = null;
    }
}