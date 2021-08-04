﻿using UnityEngine;

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
    }
}