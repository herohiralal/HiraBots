using System;
using UnityEngine;

namespace HiraBots
{
    [Serializable]
    internal partial struct LGOAPGoalLayer
    {
        internal static LGOAPGoalLayer empty => new LGOAPGoalLayer
        {
            m_Goals = new LGOAPGoal[0]
        };

        [SerializeField, HideInInspector] internal LGOAPGoal[] m_Goals;
    }
}