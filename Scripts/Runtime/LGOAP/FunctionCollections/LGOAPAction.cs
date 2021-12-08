using System.Runtime.InteropServices;
using UnityEngine;

namespace HiraBots
{
    [System.Serializable, StructLayout(LayoutKind.Auto)]
    internal partial struct LGOAPAction
    {
        internal static LGOAPAction empty => new LGOAPAction
        {
            m_Precondition = new DecoratorBlackboardFunction[0],
            m_Cost = new DecoratorBlackboardFunction[0],
            m_Effect = new EffectorBlackboardFunction[0]
        };

        [Tooltip("The conditions that must be satisfied for this task to be valid.")]
        [SerializeField, HideInInspector] internal DecoratorBlackboardFunction[] m_Precondition;

        [Tooltip("The amount of cost this task entails.")]
        [SerializeField, HideInInspector] internal DecoratorBlackboardFunction[] m_Cost;

        [Tooltip("The effect this action has upon the execution of this task.")]
        [SerializeField, HideInInspector] internal EffectorBlackboardFunction[] m_Effect;
    }
}