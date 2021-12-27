using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HiraBots
{
    [Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPGoalLayer
    {
        internal static LGOAPGoalLayer empty => new LGOAPGoalLayer
        {
            m_Goals = new LGOAPGoal[0]
        };

        [SerializeField, HideInInspector] internal ushort[] m_FallbackGoal;
        [SerializeField, HideInInspector] internal LGOAPGoal[] m_Goals;
    }
}