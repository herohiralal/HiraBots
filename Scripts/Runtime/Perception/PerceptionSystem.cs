using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.AI;

namespace HiraBots
{
    internal static partial class PerceptionSystem
    {
        private static NativeArray<int> s_SensorsStimulusMasks;
        private static HiraBotSensor[] s_Sensors;
        private static Dictionary<HiraBotSensor, int> s_SensorsLookUpTable;
        private static int s_SensorsCount;

        private static NativeArray<float4>[] s_StimuliPositions;
        private static NativeArray<int>[] s_StimuliAssociatedObjects;
        private static NativeArray<int>[] s_Stimuli;
        private static Dictionary<int, (byte type, int index)> s_StimuliLookUpTable;
        private static int[] s_StimuliCounts;

        private static int s_Id = 0;

        private static bool isActive { get; set; }

        internal static void Initialize()
        {
            if (isActive)
            {
                return;
            }

            s_Id = int.MinValue;

            s_SensorsStimulusMasks = new NativeArray<int>(8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            s_Sensors = new HiraBotSensor[8];
            s_SensorsLookUpTable = new Dictionary<HiraBotSensor, int>(8);
            s_SensorsCount = 0;

            s_StimuliPositions = new NativeArray<float4>[32];
            s_StimuliAssociatedObjects = new NativeArray<int>[32];
            s_Stimuli = new NativeArray<int>[32];
            s_StimuliLookUpTable = new Dictionary<int, (byte, int)>();
            s_StimuliCounts = new int[32];

            for (var i = 0; i < 32; i++)
            {
                s_StimuliPositions[i] = new NativeArray<float4>(0, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                s_StimuliAssociatedObjects[i] = new NativeArray<int>(0, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                s_Stimuli[i] = new NativeArray<int>(0, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                s_StimuliCounts[i] = 0;
            }

            ResetSensorsDatabaseCommandBuffer();
            ResetStimuliDatabaseCommandBuffer();

            isActive = true;
        }

        internal static void Shutdown()
        {
            if (!isActive)
            {
                return;
            }

            isActive = false;

            ApplyStimuliDatabaseCommandBuffer();
            ApplySensorsDatabaseCommandBuffer();

            foreach (var s in s_Sensors)
            {
                ShutdownSensor(s);
            }

            for (var i = 0; i < 32; i++)
            {
                s_StimuliCounts[i] = 0;
                s_Stimuli[i].Dispose();
                s_StimuliAssociatedObjects[i].Dispose();
                s_StimuliPositions[i].Dispose();
            }

            s_StimuliCounts = null;
            s_StimuliLookUpTable = null;
            s_Stimuli = null;
            s_StimuliAssociatedObjects = null;
            s_StimuliPositions = null;

            s_SensorsCount = 0;
            s_SensorsLookUpTable.Clear();
            s_Sensors = new HiraBotSensor[0];
            s_SensorsStimulusMasks.Dispose();

            s_Id = int.MinValue;
        }

        internal static void UpdateDatabase()
        {
            ApplySensorsDatabaseCommandBuffer();
            ApplyStimuliDatabaseCommandBuffer();
        }

        internal static bool shouldTick => s_SensorsCount == 0 || s_StimuliLookUpTable.Count == 0;

        internal static void ScheduleJobs(float deltaTime)
        {
            for (byte stimulusTypeIndex = 0; stimulusTypeIndex < 32; stimulusTypeIndex++)
            {
                // ignore if no stimuli of the current type
                if (s_StimuliCounts[stimulusTypeIndex] == 0)
                {
                    continue;
                }

                var type = (1 << stimulusTypeIndex);
                var stimuliPositionsForCurrentType = s_StimuliPositions[stimulusTypeIndex];
                var stimuliAssociatedObjectsForCurrentType = s_StimuliAssociatedObjects[stimulusTypeIndex];

                for (var sensorIndex = 0; sensorIndex < s_SensorsCount; sensorIndex++)
                {
                    var stimulusMask = s_SensorsStimulusMasks[sensorIndex];
                    if ((stimulusMask & type) == 0)
                    {
                        // skip this sensor if not supposed to detect
                        continue;
                    }

                    s_Sensors[sensorIndex].ScheduleJobsToDetermineObjectsPerceivedThisTick(stimuliPositionsForCurrentType, stimuliAssociatedObjectsForCurrentType);
                }
            }

            JobHandle.ScheduleBatchedJobs();

            for (var sensorIndex = 0; sensorIndex < s_SensorsCount; sensorIndex++)
            {
                s_Sensors[sensorIndex].ScheduleJobsToSortPerceivedObjectsData(deltaTime);
            }

            JobHandle.ScheduleBatchedJobs();
        }

        internal static void CollectJobResults()
        {
            for (var sensorIndex = 0; sensorIndex < s_SensorsCount; sensorIndex++)
            {
                s_Sensors[sensorIndex].CollectJobResults();
            }
        }
    }
}