using System.Collections.Generic;
using Unity.Collections;
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

        private static NativeArray<float4> s_StimuliPositions;
        private static NativeArray<int> s_StimuliTypes;
        private static NativeArray<int> s_StimuliAssociatedObjects;
        private static int s_StimuliCount;

        private static ulong s_Id = 0;

        internal static void Initialize()
        {
            s_Id = 0;

            s_SensorsStimulusMasks = new NativeArray<int>(8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            s_Sensors = new HiraBotSensor[8];
            s_SensorsLookUpTable = new Dictionary<HiraBotSensor, int>(8);
            s_SensorsCount = 0;

            s_StimuliPositions = new NativeArray<float4>(8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            s_StimuliTypes = new NativeArray<int>(8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            s_StimuliAssociatedObjects = new NativeArray<int>(8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            s_StimuliCount = 0;

            InitializeSensorsDatabaseCommandBuffer();
            InitializeStimuliDatabaseCommandBuffer();
        }

        internal static void Shutdown()
        {
            ApplyStimuliDatabaseCommandBuffer();
            ApplySensorsDatabaseCommandBuffer();

            ShutdownStimuliDatabaseCommandBuffer();
            ShutdownSensorsDatabaseCommandBuffer();

            s_StimuliCount = 0;
            s_StimuliAssociatedObjects.Dispose();
            s_StimuliTypes.Dispose();
            s_StimuliPositions.Dispose();

            s_SensorsCount = 0;
            s_SensorsLookUpTable.Clear();
            s_Sensors = new HiraBotSensor[0];
            s_SensorsStimulusMasks.Dispose();

            s_Id = 0;
        }

        internal static void UpdateDatabase()
        {
            ApplySensorsDatabaseCommandBuffer();
            ApplyStimuliDatabaseCommandBuffer();
        }

        internal static void Tick(float deltaTime)
        {
            if (s_SensorsCount == 0 && s_StimuliCount == 0)
            {
                return;
            }

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