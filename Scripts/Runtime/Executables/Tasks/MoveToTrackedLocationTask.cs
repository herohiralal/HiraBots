using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal class MoveToTrackedLocationTask : IHiraBotsTask
    {
        internal static MoveToTrackedLocationTask Get(UnityEngine.AI.BlackboardComponent blackboard,
            NavMeshAgent agent, string locationKey, float tolerance, float speed)
        {
            var output = s_Executables.Count == 0 ? new MoveToTrackedLocationTask() : s_Executables.Pop();

            output.m_Failed = false;
            output.m_Blackboard = blackboard;
            output.m_Agent = agent;
            output.m_LocationKey = locationKey;
            output.m_Tolerance = tolerance;
            output.m_Speed = speed;

            return output;
        }

        private MoveToTrackedLocationTask()
        {
        }

        private static readonly Stack<MoveToTrackedLocationTask> s_Executables = new Stack<MoveToTrackedLocationTask>();

        private UnityEngine.AI.BlackboardComponent m_Blackboard;
        private NavMeshAgent m_Agent;
        private string m_LocationKey;
        private Vector3 m_CurrentTarget;
        private float m_Tolerance;
        private float m_Speed;
        private bool m_Failed;

        public void Begin()
        {
            m_CurrentTarget = m_Blackboard.GetVectorValue(m_LocationKey);

            if (!m_Agent.SetDestination(m_CurrentTarget))
            {
                m_Failed = true;
                return;
            }

            m_Agent.speed = m_Speed;
            m_Agent.stoppingDistance = m_Tolerance;
            m_Agent.isStopped = false;
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            if (m_Failed)
            {
                return HiraBotsTaskResult.Failed;
            }

            Vector3 currentTarget = m_Blackboard.GetVectorValue(m_LocationKey);
            if (currentTarget != m_CurrentTarget)
            {
                m_CurrentTarget = currentTarget;
                if (!m_Agent.SetDestination(m_CurrentTarget))
                {
                    return HiraBotsTaskResult.Failed;
                }
            }

            if (m_Agent.remainingDistance > m_Tolerance)
            {
                return HiraBotsTaskResult.InProgress;
            }

            return HiraBotsTaskResult.Succeeded;
        }

        public void Abort()
        {
            if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
            {
                m_Agent.isStopped = true;
            }

            m_Blackboard = default;
            m_Agent = null;
            s_Executables.Push(this);
        }

        public void End(bool success)
        {
            if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
            {
                m_Agent.isStopped = true;
            }

            m_Blackboard = default;
            m_Agent = null;
            s_Executables.Push(this);
        }
    }
}