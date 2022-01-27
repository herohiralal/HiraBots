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

        private static NativeArray<float4>[] s_StimuliPositions;
        private static NativeArray<int>[] s_StimuliAssociatedObjects;
        private static NativeArray<int>[] s_Stimuli;
        private static Dictionary<int, (byte type, int index)> s_StimuliLookUpTable;
        private static int[] s_StimuliCounts;

        private static int s_Id = 0;

        internal static void Initialize()
        {
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

            InitializeSensorsDatabaseCommandBuffer();
            InitializeStimuliDatabaseCommandBuffer();
        }

        internal static void Shutdown()
        {
            ApplyStimuliDatabaseCommandBuffer();
            ApplySensorsDatabaseCommandBuffer();

            ShutdownStimuliDatabaseCommandBuffer();
            ShutdownSensorsDatabaseCommandBuffer();

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

        internal static void Tick(float deltaTime)
        {
            // if the time is frozen, ignore perception system stuff
            if (deltaTime == 0f)
            {
                return;
            }

            if (s_SensorsCount == 0 && s_StimuliLookUpTable.Count == 0)
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