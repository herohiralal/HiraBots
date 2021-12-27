using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HiraBots
{
    [Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPTaskLayer
    {
        internal static LGOAPTaskLayer empty => new LGOAPTaskLayer
        {
            m_MaxPlanSize = 5,
            m_FallbackPlan = new ushort[1],
            m_Tasks = new LGOAPTask[0]
        };

        [SerializeField, HideInInspector] internal ushort[] m_FallbackPlan;
        [SerializeField, HideInInspector] internal LGOAPTask[] m_Tasks;

        [Tooltip("The max size of a plan at this layer.")]
        [SerializeField, HideInInspector] internal byte m_MaxPlanSize;
    }
}