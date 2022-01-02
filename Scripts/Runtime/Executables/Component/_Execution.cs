using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    internal partial class ExecutorComponent
    {
        private IHiraBotsTask m_CurrentTask;

        internal bool hasTask => m_CurrentTask != null;

        internal bool? lastTaskEndSucceeded { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void BeginTask(IHiraBotsTask task)
        {
            m_CurrentTask = task;
            m_CurrentTask.WrappedBegin();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AbortTask()
        {
            m_CurrentTask.WrappedAbort();
            m_CurrentTask = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Tick(float deltaTime)
        {
            switch (m_CurrentTask.WrappedExecute(deltaTime))
            {
                case HiraBotsTaskResult.InProgress:
                    return true;

                case HiraBotsTaskResult.Succeeded:
                    m_CurrentTask.WrappedEnd(true);
                    m_CurrentTask = null;
                    lastTaskEndSucceeded = true;
                    return false;

                case HiraBotsTaskResult.Failed:
                    m_CurrentTask.WrappedEnd(false);
                    m_CurrentTask = null;
                    lastTaskEndSucceeded = false;
                    return false;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}