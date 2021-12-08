using System.Runtime.InteropServices;
using UnityEngine;

namespace HiraBots
{
    [System.Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPTarget
    {
        internal static LGOAPTarget empty => new LGOAPTarget
        {
            m_Target = new DecoratorBlackboardFunction[0]
        };

        [Tooltip("The conditions that must be satisfied for this goal/task to be fulfilled.")]
        [SerializeField, HideInInspector] internal DecoratorBlackboardFunction[] m_Target;
    }
}