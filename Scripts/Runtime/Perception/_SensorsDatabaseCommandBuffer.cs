using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal static partial class PerceptionSystem
    {
        private struct AddSensorCommand
        {
            internal AddSensorCommand(HiraBotSensor sensor, int stimulusMask)
            {
                this.sensor = sensor;
                this.stimulusMask = stimulusMask;
            }

            internal HiraBotSensor sensor { get; }
            internal int stimulusMask { get; }
        }

        private struct RemoveSensorCommand
        {
            internal RemoveSensorCommand(HiraBotSensor sensor)
            {
                this.sensor = sensor;
            }

            internal HiraBotSensor sensor { get; }
        }

        private struct ChangeSensorStimulusMaskCommand
        {
            internal ChangeSensorStimulusMaskCommand(HiraBotSensor sensor, int newStimulusMask)
            {
                this.sensor = sensor;
                this.newStimulusMask = newStimulusMask;
            }

            internal HiraBotSensor sensor { get; }
            internal int newStimulusMask { get; }
        }

        private enum SensorsDatabaseCommandType : byte
        {
            Add,
            Remove,
            ChangeStimulusMask
        }

        private static Queue<SensorsDatabaseCommandType> s_SensorsDatabaseCommandTypes;

        private static Queue<AddSensorCommand> s_AddSensorCommands;
        private static Queue<RemoveSensorCommand> s_RemoveSensorCommands;
        private static Queue<ChangeSensorStimulusMaskCommand> s_ChangeSensorStimulusMaskCommands;

        private static void InitializeSensorsDatabaseCommandBuffer()
        {
            s_SensorsDatabaseCommandTypes = new Queue<SensorsDatabaseCommandType>();

            s_AddSensorCommands = new Queue<AddSensorCommand>();
            s_RemoveSensorCommands = new Queue<RemoveSensorCommand>();
            s_ChangeSensorStimulusMaskCommands = new Queue<ChangeSensorStimulusMaskCommand>();
        }

        private static void ShutdownSensorsDatabaseCommandBuffer()
        {
            s_ChangeSensorStimulusMaskCommands = null;
            s_RemoveSensorCommands = null;
            s_AddSensorCommands = null;

            s_SensorsDatabaseCommandTypes = null;
        }

        internal static void AddSensor(HiraBotSensor sensor, int stimulusMask)
        {
            if (!isActive)
            {
                return;
            }

            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.Add);
            s_AddSensorCommands.Enqueue(new AddSensorCommand(sensor, stimulusMask));
        }

        internal static void RemoveSensor(HiraBotSensor sensor)
        {
            if (!isActive)
            {
                return;
            }

            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.Remove);
            s_RemoveSensorCommands.Enqueue(new RemoveSensorCommand(sensor));
        }

        internal static void ChangeStimulusMask(HiraBotSensor sensor, int newStimulusMask)
        {
            if (!isActive)
            {
                return;
            }

            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.ChangeStimulusMask);
            s_ChangeSensorStimulusMaskCommands.Enqueue(new ChangeSensorStimulusMaskCommand(sensor, newStimulusMask));
        }

        private static void ApplySensorsDatabaseCommandBuffer()
        {
            while (s_SensorsDatabaseCommandTypes.Count > 0)
            {
                switch (s_SensorsDatabaseCommandTypes.Dequeue())
                {
                    case SensorsDatabaseCommandType.Add:
                    {
                        var cmd = s_AddSensorCommands.Dequeue();
                        if (s_SensorsLookUpTable.ContainsKey(cmd.sensor))
                        {
                            break;
                        }

                        if (s_SensorsCount == s_Sensors.Length)
                        {
                            // reallocation time
                            s_SensorsStimulusMasks.Reallocate(s_SensorsCount + 8, Allocator.Persistent,
                                NativeArrayOptions.UninitializedMemory);

                            System.Array.Resize(ref s_Sensors, s_SensorsCount + 8);
                        }

                        s_SensorsStimulusMasks[s_SensorsCount] = cmd.stimulusMask;

                        s_SensorsLookUpTable.Add(cmd.sensor, s_SensorsCount);

                        s_Sensors[s_SensorsCount] = cmd.sensor;

                        s_SensorsCount++;

                        cmd.sensor.Initialize();

                        break;
                    }
                    case SensorsDatabaseCommandType.Remove:
                    {
                        var cmd = s_RemoveSensorCommands.Dequeue();
                        if (!s_SensorsLookUpTable.TryGetValue(cmd.sensor, out var indexToRemove))
                        {
                            break;
                        }

                        cmd.sensor.Shutdown();

                        var lastSensorIndex = s_SensorsCount - 1;
                        var lastSensor = s_Sensors[lastSensorIndex];

                        s_SensorsStimulusMasks[indexToRemove] = s_SensorsStimulusMasks[lastSensorIndex];

                        s_SensorsLookUpTable[lastSensor] = indexToRemove;
                        s_SensorsLookUpTable.Remove(cmd.sensor);

                        s_Sensors[indexToRemove] = lastSensor;
                        s_Sensors[lastSensorIndex] = null;

                        s_SensorsCount--;

                        break;
                    }
                    case SensorsDatabaseCommandType.ChangeStimulusMask:
                    {
                        var cmd = s_ChangeSensorStimulusMaskCommands.Dequeue();
                        if (!s_SensorsLookUpTable.TryGetValue(cmd.sensor, out var indexToChange))
                        {
                            break;
                        }

                        s_SensorsStimulusMasks[indexToChange] = cmd.newStimulusMask;
                        break;
                    }
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }

            if (s_SensorsDatabaseCommandTypes.Count == 0
                && s_SensorsDatabaseCommandTypes.Count == s_AddSensorCommands.Count
                && s_AddSensorCommands.Count == s_RemoveSensorCommands.Count
                && s_RemoveSensorCommands.Count == s_ChangeSensorStimulusMaskCommands.Count)
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