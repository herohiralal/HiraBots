using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal abstract class MoveToTrackedLocationTask<T> : IHiraBotsTask where T : MoveToTrackedLocationTask<T>, new()
    {
        internal static IHiraBotsTask Get(UnityEngine.AI.BlackboardComponent blackboard,
            NavMeshAgent agent, string locationKey, float tolerance, float speed)
        {
            var output = s_Executables.Count == 0 ? new T() : s_Executables.Pop();

            output.m_Failed = false;
            output.m_Blackboard = blackboard;
            output.m_Agent = agent;
            output.m_LocationKey = locationKey;
            output.m_Tolerance = tolerance;
            output.m_Speed = speed;

            return output;
        }

        private static readonly Stack<T> s_Executables = new Stack<T>();

        private UnityEngine.AI.BlackboardComponent m_Blackboard;
        private NavMeshAgent m_Agent;
        private string m_LocationKey;
        private Vector3 m_CurrentTarget;
        private float m_Tolerance;
        private float m_Speed;
        private bool m_Failed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool TryGetLocation(UnityEngine.AI.BlackboardComponent blackboard, string key, out Vector3 location);

        public void Begin()
        {
            if (!TryGetLocation(m_Blackboard, m_LocationKey, out m_CurrentTarget))
            {
                m_Failed = true;
                return;
            }

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

            if (!TryGetLocation(m_Blackboard, m_LocationKey, out var currentTarget))
            {
                return HiraBotsTaskResult.Failed;
            }

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
            s_Executables.Push((T) this);
        }

        public void End(bool success)
        {
            if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
            {
                m_Agent.isStopped = true;
            }

            m_Blackboard = default;
            m_Agent = null;
            s_Executables.Push((T) this);
        }
    }

    internal sealed class MoveToTrackedVectorTask : MoveToTrackedLocationTask<MoveToTrackedVectorTask>
    {
        protected override bool TryGetLocation(UnityEngine.AI.BlackboardComponent blackboard, string key, out Vector3 location)
        {
            location = blackboard.GetVectorValue(key);
            return true;
        }
    }

    internal sealed class MoveToTrackedGameObjectTask : MoveToTrackedLocationTask<MoveToTrackedGameObjectTask>
    {
        protected override bool TryGetLocation(UnityEngine.AI.BlackboardComponent blackboard, string key, out Vector3 location)
        {
            if (!(blackboard.GetObjectValue(key) is GameObject go) || go == null)
            {
                location = Vector3.zero;
                return false;
            }

            location = go.transform.position;
            return true;
        }
    }

    internal sealed class MoveToTrackedTransformTask : MoveToTrackedLocationTask<MoveToTrackedTransformTask>
    {
        protected override bool TryGetLocation(UnityEngine.AI.BlackboardComponent blackboard, string key, out Vector3 location)
        {
            if (!(blackboard.GetObjectValue(key) is Transform t) || t == null)
            {
                location = Vector3.zero;
                return false;
            }

            location = t.position;
            return true;
        }
    }

    internal sealed class MoveToTrackedComponentTask : MoveToTrackedLocationTask<MoveToTrackedComponentTask>
    {
        protected override bool TryGetLocation(UnityEngine.AI.BlackboardComponent blackboard, string key, out Vector3 location)
        {
            if (!(blackboard.GetObjectValue(key) is Component c) || c == null)
            {
                location = Vector3.zero;
                return false;
            }

            location = c.transform.position;
            return true;
        }
    }
}