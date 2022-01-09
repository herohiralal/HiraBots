using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine
{
    public partial class TacMap
    {
        public delegate JobHandle ScheduleJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions, float cellSize, JobHandle dependencies);

        public delegate void RunJobDelegate(NativeArray<float> map, int3 pivot, int3 dimensions, float cellSize);

        private bool ValidateInput<T>(T del)
        {
            if (m_TacMapComponent == null)
            {
                Debug.LogError("Attempting to access the data of a disabled tac map.", this);
                return false;
            }

            if (del == null)
            {
                Debug.LogException(new System.NullReferenceException("Parameter null: 'del'"), this);
                return false;
            }

            return true;
        }

        public JobHandle RequestDataForWriteJob(ScheduleJobDelegate del)
        {
            return ValidateInput(del) ? m_TacMapComponent.RequestDataForWriteJob(del.Invoke) : default;
        }

        public JobHandle RequestDataForReadJob(ScheduleJobDelegate del)
        {
            return ValidateInput(del) ? m_TacMapComponent.RequestDataForReadJob(del.Invoke) : default;
        }

        public void RequestDataForWriteJob(RunJobDelegate del)
        {
            if (ValidateInput(del)) m_TacMapComponent.RequestDataForWriteJob(del.Invoke);
        }

        public void RequestDataForReadJob(RunJobDelegate del)
        {
            if (ValidateInput(del)) m_TacMapComponent.RequestDataForReadJob(del.Invoke);
        }
    }
}