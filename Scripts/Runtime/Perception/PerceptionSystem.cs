using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace HiraBots
{
    internal static class PerceptionSystem
    {
        private static SensorComponent[] s_Sensors;
        private static Dictionary<SensorComponent, int> s_SensorsLookUpTable;
        private static int s_SensorsCount;

        private static NativeArray<float3> s_StimuliPositions;
        private static NativeArray<int> s_AssociatedGameObjects;
        private static int s_StimuliCount;

        private static ulong s_Id = 0;

        internal static void Initialize()
        {
            s_Id = 0;
        }

        internal static void Shutdown()
        {
            s_Id = 0;
        }

        internal static unsafe void Tick(float deltaTime)
        {
            // for all the stimuli
            // for all the sensors
            // check if the masks match and schedule a bounds check job
            // can probably run main module ticking job here

            // synchronize and make sure all bounds check jobs are done
            // once the bounds have been figured out, start LOS check and nav-distance check jobs
            // for the ones that require them
            // for the ones that don't, update their 'perceived game objects' sets

            // complete all LOS (or nav-distance but not both) check jobs and update their
            // perceived game object sets

            // do the same with the other one
        }
    }
}