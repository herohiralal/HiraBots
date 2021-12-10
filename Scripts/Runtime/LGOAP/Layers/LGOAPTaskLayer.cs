using System;
using UnityEngine;

namespace HiraBots
{
    [Serializable]
    internal partial struct LGOAPTaskLayer
    {
        internal static LGOAPTaskLayer empty => new LGOAPTaskLayer
        {
            m_Tasks = new LGOAPTask[0]
        };

        [SerializeField, HideInInspector] internal LGOAPTask[] m_Tasks;
    }
}