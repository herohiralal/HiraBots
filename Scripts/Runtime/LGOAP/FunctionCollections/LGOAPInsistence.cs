using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    [System.Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPInsistence
    {
        internal static LGOAPInsistence empty => new LGOAPInsistence
        {
            m_Insistence = new HiraBotsScoreCalculatorBlackboardFunction[0]
        };

        [Tooltip("The amount of insistence this goal has.")]
        [SerializeField, HideInInspector] internal HiraBotsScoreCalculatorBlackboardFunction[] m_Insistence;
    }
}