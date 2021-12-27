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
            m_FallbackPlan = new ushort[1],
            m_Tasks = new LGOAPTask[0]
        };

        [SerializeField, HideInInspector] internal ushort[] m_FallbackPlan;
        [SerializeField, HideInInspector] internal LGOAPTask[] m_Tasks;
    }
}