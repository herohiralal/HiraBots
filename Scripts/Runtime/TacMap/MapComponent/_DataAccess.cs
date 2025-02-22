using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace HiraBots
{
    internal delegate JobHandle ScheduleTacMapJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions, float cellSize, JobHandle dependencies);

    internal delegate void RunTacMapJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions, float cellSize);

    internal partial class TacMapComponent
    {
        internal NativeArray<float> map => m_Internal;

        internal int3 pivot => m_Pivot;

        internal int3 dimensions => m_Dimensions;

        internal float cellSize => m_CellSize;

        internal JobHandle writeJobDependencies =>
            m_ActiveWriteJob.HasValue && m_ActiveReadJob.HasValue
                ? JobHandle.CombineDependencies(m_ActiveWriteJob.Value, m_ActiveReadJob.Value)
                : m_ActiveReadJob ?? (m_ActiveWriteJob ?? default); // run after all active jobs

        internal void UpdateLatestWriteJob(JobHandle jh)
        {
            m_ActiveWriteJob = jh;

            if (m_WriteJobTrackerCoroutine == null && !jh.IsCompleted)
            {
                m_WriteJobTrackerCoroutine = CoroutineRunner.Start(WaitForWriteJobToCompleteCoroutine());
            }
        }

        internal JobHandle readJobDependencies =>
            m_ActiveWriteJob ?? default; // run after a write job, if any

        internal void UpdateLatestReadJob(JobHandle jh)
        {
            m_ActiveReadJob = jh;

            if (m_ReadJobTrackerCoroutine == null && !jh.IsCompleted)
            {
                m_ReadJobTrackerCoroutine = CoroutineRunner.Start(WaitForReadJobToCompleteCoroutine());
            }
        }

        internal void CompleteAllWriteJobDependencies()
        {
            // run after all jobs
            m_ActiveWriteJob?.Complete();
            m_ActiveWriteJob = null;

            m_ActiveReadJob?.Complete();
            m_ActiveReadJob = null;

            if (m_WriteJobTrackerCoroutine != null)
            {
                CoroutineRunner.Stop(m_WriteJobTrackerCoroutine);
                m_WriteJobTrackerCoroutine = null;
            }

            if (m_ReadJobTrackerCoroutine != null)
            {
                CoroutineRunner.Stop(m_ReadJobTrackerCoroutine);
                m_ReadJobTrackerCoroutine = null;
            }
        }

        internal void CompleteAllReadJobDependencies()
        {
            // run after write job
            m_ActiveWriteJob?.Complete();
            m_ActiveWriteJob = null;

            if (m_WriteJobTrackerCoroutine != null)
            {
                CoroutineRunner.Stop(m_WriteJobTrackerCoroutine);
                m_WriteJobTrackerCoroutine = null;
            }
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