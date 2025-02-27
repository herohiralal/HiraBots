﻿using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    [System.Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPTarget
    {
        internal static LGOAPTarget empty => new LGOAPTarget
        {
            m_Target = new HiraBotsDecoratorBlackboardFunction[0]
        };

        [Tooltip("The conditions that must be satisfied for this goal/task to be fulfilled.")]
        [SerializeField, HideInInspector] internal HiraBotsDecoratorBlackboardFunction[] m_Target;
    }
}