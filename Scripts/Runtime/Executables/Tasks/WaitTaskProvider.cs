﻿using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal sealed partial class WaitTaskProvider : HiraBotsTaskProvider
    {
        [Tooltip("The amount of time to wait.")]
        [SerializeField] private float m_Timer;

        [Tooltip("Optional random deviation over the time to wait for.")]
        [SerializeField] private float m_RandomDeviation;

        protected override IHiraBotsTask GetTask(UnityEngine.AI.BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return m_RandomDeviation != 0
                ? WaitTask.Get(m_Timer + Random.Range(-m_RandomDeviation, m_RandomDeviation))
                : WaitTask.Get(m_Timer);
        }
    }
}