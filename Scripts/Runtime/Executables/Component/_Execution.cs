using UnityEngine;

namespace HiraBots
{
    internal partial class ExecutorComponent
    {
        private IHiraBotsTask m_CurrentTask;

        internal bool hasTask => m_CurrentTask != null;

        internal void Execute(IHiraBotsTask task, float tickInterval)
        {
            if (hasTask)
            {
                HiraBotsTaskRunner.Remove(this);
            }

            m_CurrentTask = task;

            HiraBotsTaskRunner.Add(this, tickInterval);
        }

        internal void BeginTask()
        {
            try
            {
                m_CurrentTask.Begin();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal void AbortTask()
        {
            try
            {
                m_CurrentTask.Abort();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            m_CurrentTask = null;
        }

        internal bool Tick(float deltaTime)
        {
            var result = HiraBotsTaskResult.InProgress;

            try
            {
                result = m_CurrentTask.Execute(deltaTime);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            switch (result)
            {
                case HiraBotsTaskResult.InProgress:
                    return true;

                case HiraBotsTaskResult.Succeeded:
                    try
                    {
                        m_CurrentTask.End(true);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }

                    m_CurrentTask = null;
                    return false;

                case HiraBotsTaskResult.Failed:
                    try
                    {
                        m_CurrentTask.End(false);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }

                    m_CurrentTask = null;
                    return false;

                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}