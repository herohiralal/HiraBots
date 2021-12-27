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
            m_MaxPlanSize = 1,
            m_FallbackGoal = new ushort[1],
            m_Goals = new LGOAPGoal[0]
        };

        [SerializeField, HideInInspector] internal ushort[] m_FallbackGoal;
        [SerializeField, HideInInspector] internal LGOAPGoal[] m_Goals;

        [Tooltip("The max size of a plan at this layer.")]
        [SerializeField, HideInInspector] internal byte m_MaxPlanSize;
    }
}