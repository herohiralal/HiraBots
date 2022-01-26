using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    [System.Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPAction
    {
        internal static LGOAPAction empty => new LGOAPAction
        {
            m_Precondition = new HiraBotsDecoratorBlackboardFunction[0],
            m_Cost = new HiraBotsScoreCalculatorBlackboardFunction[0],
            m_Effect = new HiraBotsEffectorBlackboardFunction[0]
        };

        [Tooltip("The conditions that must be satisfied for this task to be valid.")]
        [SerializeField, HideInInspector] internal HiraBotsDecoratorBlackboardFunction[] m_Precondition;

        [Tooltip("The amount of cost this task entails.")]
        [SerializeField, HideInInspector] internal HiraBotsScoreCalculatorBlackboardFunction[] m_Cost;

        [Tooltip("The effect this action has upon the execution of this task.")]
        [SerializeField, HideInInspector] internal HiraBotsEffectorBlackboardFunction[] m_Effect;
    }
}