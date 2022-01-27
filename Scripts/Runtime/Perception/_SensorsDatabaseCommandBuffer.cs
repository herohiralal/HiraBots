using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal static partial class PerceptionSystem
    {
        private enum SensorsDatabaseCommandType : byte
        {
            Add,
            Remove,
            ChangeStimulusMask
        }

        private static Queue<SensorsDatabaseCommandType> s_SensorsDatabaseCommandTypes;
        private static Queue<HiraBotSensor> s_BufferedSensors;

        private static void InitializeSensorsDatabaseCommandBuffer()
        {
            s_SensorsDatabaseCommandTypes = new Queue<SensorsDatabaseCommandType>();
            s_BufferedSensors = new Queue<HiraBotSensor>();
        }

        private static void ShutdownSensorsDatabaseCommandBuffer()
        {
            s_BufferedSensors = null;
            s_SensorsDatabaseCommandTypes = null;
        }

        internal static void AddSensor(HiraBotSensor sensor)
        {
            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.Add);
            s_BufferedSensors.Enqueue(sensor);
        }

        internal static void RemoveSensor(HiraBotSensor sensor)
        {
            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.Remove);
            s_BufferedSensors.Enqueue(sensor);
        }

        internal static void ChangeStimulusMask(HiraBotSensor sensor)
        {
            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.ChangeStimulusMask);
            s_BufferedSensors.Enqueue(sensor);
        }

        private static void ApplySensorsDatabaseCommandBuffer()
        {
            while (s_SensorsDatabaseCommandTypes.Count > 0)
            {
                switch (s_SensorsDatabaseCommandTypes.Dequeue())
                {
                    case SensorsDatabaseCommandType.Add:
                    {
                        var relatedSensor = s_BufferedSensors.Dequeue();
                        if (s_SensorsLookUpTable.ContainsKey(relatedSensor))
                        {
                            break;
                        }

                        if (s_SensorsCount == s_Sensors.Length)
                        {
                            // reallocation time
                            s_SensorsStimulusMasks.Reallocate(s_SensorsCount + 8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                            System.Array.Resize(ref s_Sensors, s_SensorsCount + 8);
                        }

                        s_SensorsStimulusMasks[s_SensorsCount] = relatedSensor.stimulusMask;

                        s_SensorsLookUpTable.Add(relatedSensor, s_SensorsCount);

                        s_Sensors[s_SensorsCount] = relatedSensor;

                        s_SensorsCount++;
                        break;
                    }
                    case SensorsDatabaseCommandType.Remove:
                    {
                        var relatedSensor = s_BufferedSensors.Dequeue();
                        if (!s_SensorsLookUpTable.TryGetValue(relatedSensor, out var indexToRemove))
                        {
                            break;
                        }

                        var lastSensorIndex = s_SensorsCount - 1;
                        var lastSensor = s_Sensors[lastSensorIndex];

                        s_SensorsStimulusMasks[indexToRemove] = s_SensorsStimulusMasks[lastSensorIndex];

                        s_SensorsLookUpTable[lastSensor] = indexToRemove;
                        s_SensorsLookUpTable.Remove(relatedSensor);

                        s_Sensors[indexToRemove] = lastSensor;
                        s_Sensors[lastSensorIndex] = null;

                        s_SensorsCount--;

                        break;
                    }
                    case SensorsDatabaseCommandType.ChangeStimulusMask:
                    {
                        var relatedSensor = s_BufferedSensors.Dequeue();
                        if (!s_SensorsLookUpTable.TryGetValue(relatedSensor, out var indexToChange))
                        {
                            break;
                        }

                        s_SensorsStimulusMasks[indexToChange] = relatedSensor.stimulusMask;
                        break;
                    }
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }

            if (s_SensorsDatabaseCommandTypes.Count == 0
                && s_SensorsDatabaseCommandTypes.Count == s_BufferedSensors.Count)
            {
                return;
            }

#if HIRA_BOTS_CREATOR_MODE
            Debug.Break();
            Debug.LogError("Command buffers not correctly cleared in Perception System sensors database.");
#endif
        }
    }
}