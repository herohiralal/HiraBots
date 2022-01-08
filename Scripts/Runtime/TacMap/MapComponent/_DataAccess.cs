using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal delegate JobHandle ScheduleTacMapJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions, JobHandle dependencies);

    internal delegate void RunTacMapJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions);

    internal partial class TacMapComponent
    {
        internal JobHandle RequestDataForWriteJob(ScheduleTacMapJobDelegate scheduleJobDelegate)
        {
            var dependencies = m_ActiveWriteJob.HasValue && m_ActiveReadJob.HasValue
                ? JobHandle.CombineDependencies(m_ActiveWriteJob.Value, m_ActiveReadJob.Value)
                : m_ActiveReadJob ?? (m_ActiveWriteJob ?? default); // run after all active jobs

            try
            {
                m_ActiveWriteJob = scheduleJobDelegate.Invoke(m_Internal, m_Pivot, m_Dimensions, dependencies);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return default;
            }

            if (m_WriteJobTrackerCoroutine == null)
            {
                m_WriteJobTrackerCoroutine = CoroutineRunner.Start(WaitForWriteJobToCompleteCoroutine());
            }

            return m_ActiveWriteJob ?? default;
        }

        internal JobHandle RequestDataForReadJob(ScheduleTacMapJobDelegate scheduleJobDelegate)
        {
            var dependencies = m_ActiveWriteJob ?? default; // run after a write job, if any

            try
            {
                m_ActiveReadJob = scheduleJobDelegate.Invoke(m_Internal, m_Pivot, m_Dimensions, dependencies);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return default;
            }

            if (m_ReadJobTrackerCoroutine == null)
            {
                m_ReadJobTrackerCoroutine = CoroutineRunner.Start(WaitForReadJobToCompleteCoroutine());
            }

            return m_ActiveReadJob ?? default;
        }

        internal void RequestDataForWriteJob(RunTacMapJobDelegate runJobDelegate)
        {
            // run after all jobs
            m_ActiveWriteJob?.Complete();
            m_ActiveWriteJob = null;

            m_ActiveReadJob?.Complete();
            m_ActiveReadJob = null;

            runJobDelegate.Invoke(m_Internal, m_Pivot, m_Dimensions);
        }

        internal void RequestDataForReadJob(RunTacMapJobDelegate runJobDelegate)
        {
            // run after write job
            m_ActiveWriteJob?.Complete();
            m_ActiveWriteJob = null;

            runJobDelegate.Invoke(m_Internal, m_Pivot, m_Dimensions);
        }

        private IEnumerator WaitForWriteJobToCompleteCoroutine()
        {

            yield return null;
            while (m_ActiveWriteJob.HasValue && !m_ActiveWriteJob.Value.IsCompleted)
            {
                yield return null;
            }

            m_ActiveWriteJob?.Complete();
            m_ActiveWriteJob = null;

            m_WriteJobTrackerCoroutine = null;
        }

        private IEnumerator WaitForReadJobToCompleteCoroutine()
        {

            yield return null;
            while (m_ActiveReadJob.HasValue && !m_ActiveReadJob.Value.IsCompleted)
            {
                yield return null;
            }

            m_ActiveReadJob?.Complete();
            m_ActiveReadJob = null;

            m_ReadJobTrackerCoroutine = null;
        }
    }
}