using System.Collections.Generic;
using UnityEngine.AI;

namespace HiraBots
{
    internal class FailTask : IHiraBotsTask
    {
        internal static FailTask Get()
        {
            return s_Executables.Count == 0 ? new FailTask() : s_Executables.Pop();
        }

        private FailTask()
        {
        }

        private static readonly Stack<FailTask> s_Executables = new Stack<FailTask>();

        public void Begin()
        {
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            return HiraBotsTaskResult.Failed;
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