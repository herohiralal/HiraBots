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

        [SerializeField] private LGOAPGoal[] m_Goals;

        internal void PrepareForCompilation()
        {
            foreach (var goal in m_Goals)
            {
                goal.PrepareForCompilation();
            }
        }
    }

    [Serializable]
    internal partial struct LGOAPTaskLayer
    {
        internal static LGOAPTaskLayer empty => new LGOAPTaskLayer
        {
            m_Tasks = new LGOAPTask[0]
        };

        [SerializeField] private LGOAPTask[] m_Tasks;

        internal void PrepareForCompilation()
        {
            foreach (var task in m_Tasks)
            {
                task.PrepareForCompilation();
            }
        }
    }
}