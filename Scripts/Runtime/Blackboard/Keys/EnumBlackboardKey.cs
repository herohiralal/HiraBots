﻿using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// 8-bit unsigned Enum blackboard key.
    /// </summary>
    internal partial class EnumBlackboardKey : BlackboardKey
    {
        internal EnumBlackboardKey()
        {
            m_SizeInBytes = sizeof(byte);
            m_KeyType = BlackboardKeyType.Enum;
        }

        [Tooltip("The default value for this key that a blackboard would start with.")]
        [SerializeField] private DynamicEnum m_DefaultValue = default;

#if UNITY_EDITOR
        /// <summary>
        /// (EDITOR-ONLY) The type identifier associated with this enum key.
        /// </summary>
        internal string typeIdentifier => m_DefaultValue.m_TypeIdentifier;
#endif
    }
}