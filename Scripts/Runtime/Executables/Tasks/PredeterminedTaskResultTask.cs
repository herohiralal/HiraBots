using System.Collections.Generic;
using UnityEngine.AI;

namespace HiraBots
{
    internal class PredeterminedTaskResultTask : IHiraBotsTask
    {
        internal static PredeterminedTaskResultTask Get(HiraBotsTaskResult result)
        {
            var output = s_Executables.Count == 0 ? new PredeterminedTaskResultTask() : s_Executables.Pop();
            output.m_Result = result;
            return output;
        }

        private PredeterminedTaskResultTask()
        {
        }

        private HiraBotsTaskResult m_Result;

        private static readonly Stack<PredeterminedTaskResultTask> s_Executables = new Stack<PredeterminedTaskResultTask>();

        public void Begin()
        {
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            return m_Result;
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