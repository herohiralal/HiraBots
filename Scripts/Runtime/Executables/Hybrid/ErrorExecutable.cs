using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    internal sealed class ErrorExecutable : IHiraBotsTask, IHiraBotsService
    {
        internal static ErrorExecutable Get(string error)
        {
            var output = s_Executables.Count == 0 ? new ErrorExecutable() : s_Executables.Pop();

            output.m_Error = error;

            return output;
        }

        private ErrorExecutable()
        {
            m_Error = "";
        }

        private static readonly Stack<ErrorExecutable> s_Executables = new Stack<ErrorExecutable>();

        private string m_Error;

        public void Begin()
        {
            Debug.LogError($"Error running task: {m_Error}.");
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

        public void Start()
        {
            Debug.LogError($"Error running service: {m_Error}.");
        }

        public void Tick(float deltaTime)
        {
        }

        public void Stop()
        {
            s_Executables.Push(this);
        }
    }
}