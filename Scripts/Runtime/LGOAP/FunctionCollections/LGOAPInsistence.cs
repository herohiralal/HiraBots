using System.Runtime.InteropServices;
using UnityEngine;

namespace HiraBots
{
    [System.Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPInsistence
    {
        internal static LGOAPInsistence empty => new LGOAPInsistence
        {
            m_Insistence = new DecoratorBlackboardFunction[0]
        };

        [Tooltip("The amount of insistence this goal has.")]
        [SerializeField, HideInInspector] internal DecoratorBlackboardFunction[] m_Insistence;
    }
}