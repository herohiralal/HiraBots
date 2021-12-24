using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    internal sealed class WaitTask : IHiraBotsTask
    {
        internal static WaitTask Get(float timer)
        {
            var output = s_Executables.Count == 0 ? new WaitTask() : s_Executables.Pop();

            output.m_WaitTimer = timer;

            return output;
        }

        private WaitTask()
        {
            m_WaitTimer = float.MaxValue;
        }

        private static readonly Stack<WaitTask> s_Executables = new Stack<WaitTask>();

        private float m_WaitTimer;

        public void Begin()
        {
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            m_WaitTimer -= deltaTime;

            return m_WaitTimer > 0f ? HiraBotsTaskResult.InProgress : HiraBotsTaskResult.Succeeded;
        }

        public void Abort()
        {
            s_Executables.Push(this);
        }

        public void End(bool success)
        {
            s_Executables.Push(this);
        }
    }
}