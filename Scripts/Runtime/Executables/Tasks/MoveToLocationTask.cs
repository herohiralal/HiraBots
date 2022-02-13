using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal class MoveToLocationTask : IHiraBotsTask
    {
        internal static MoveToLocationTask Get(NavMeshAgent agent, Vector3 location, float tolerance, float speed)
        {
            var output = s_Executables.Count == 0 ? new MoveToLocationTask() : s_Executables.Pop();

            output.m_Failed = false;
            output.m_Agent = agent;
            output.m_Location = location;
            output.m_Tolerance = tolerance;
            output.m_Speed = speed;

            return output;
        }

        private MoveToLocationTask()
        {
        }

        private static readonly Stack<MoveToLocationTask> s_Executables = new Stack<MoveToLocationTask>();

        private NavMeshAgent m_Agent;
        private Vector3 m_Location;
        private float m_Tolerance;
        private float m_Speed;
        private bool m_Failed;

        public void Begin()
        {
            if (!NavMesh.SamplePosition(m_Location, out var hit, m_Tolerance / 2, NavMesh.AllAreas))
            {
                m_Failed = true;
                return;
            }

            var path = new NavMeshPath();
            m_Agent.CalculatePath(hit.position, path);

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                m_Failed = true;
                return;
            }

            m_Agent.SetPath(path);
            m_Agent.speed = m_Speed;
            m_Agent.stoppingDistance = m_Tolerance;
            m_Agent.isStopped = false;
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            return m_Failed
                ? HiraBotsTaskResult.Failed
                : m_Agent.remainingDistance <= m_Tolerance
                    ? HiraBotsTaskResult.Succeeded
                    : HiraBotsTaskResult.InProgress;
        }

        public void Abort()
        {
            if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
            {
                m_Agent.isStopped = true;
            }

            m_Agent = null;
            s_Executables.Push(this);
        }

        public void End(bool success)
        {
            if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
            {
                m_Agent.isStopped = true;
            }

            m_Agent = null;
            s_Executables.Push(this);
        }
    }
}