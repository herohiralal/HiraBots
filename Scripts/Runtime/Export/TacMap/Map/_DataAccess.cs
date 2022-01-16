using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine
{
    public partial class TacMap
    {
        public delegate JobHandle ScheduleJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions, float cellSize, JobHandle dependencies);

        public delegate void RunJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions, float cellSize);

        private bool Validate()
        {
            if (m_TacMapComponent == null)
            {
                Debug.LogError("Attempting to access the data of a disabled tac map.", this);
                return false;
            }

            return true;
        }

        public JobHandle writeJobDependencies => Validate()
            ? m_TacMapComponent.writeJobDependencies
            : default;

        public void UpdateLatestWriteJob(JobHandle jh)
        {
            if (!Validate())
            {
                return;
            }

            m_TacMapComponent.UpdateLatestWriteJob(jh);
        }

        public void CompleteAllWriteJobDependencies()
        {
            if (!Validate())
            {
                return;
            }

            m_TacMapComponent.CompleteAllWriteJobDependencies();
        }

        public JobHandle readJobDependencies => Validate()
            ? m_TacMapComponent.readJobDependencies
            : default;

        public void UpdateLatestReadJob(JobHandle jh)
        {
            if (!Validate())
            {
                return;
            }

            m_TacMapComponent.UpdateLatestReadJob(jh);
        }

        public void CompleteAllReadJobDependencies()
        {
            if (!Validate())
            {
                return;
            }

            m_TacMapComponent.CompleteAllReadJobDependencies();
        }
    }
}