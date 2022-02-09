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
            internal AddSensorCommand(HiraBotSensor sensor, int stimulusMask, SensorSecondaryChecksFlags secondaryCheckFlags)
            {
                this.sensor = sensor;
                this.stimulusMask = stimulusMask;
                this.secondaryCheckFlags = secondaryCheckFlags;
            }

            internal HiraBotSensor sensor { get; }
            internal int stimulusMask { get; }
            internal SensorSecondaryChecksFlags secondaryCheckFlags { get; }
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

        private struct ChangeSensorSecondaryCheckEnabledCommand
        {
            internal ChangeSensorSecondaryCheckEnabledCommand(HiraBotSensor sensor, SensorSecondaryChecksFlags secondaryChecksFlags)
            {
                this.sensor = sensor;
                this.secondaryChecksFlags = secondaryChecksFlags;
            }

            internal HiraBotSensor sensor { get; }
            internal SensorSecondaryChecksFlags secondaryChecksFlags { get; }
        }

        private enum SensorsDatabaseCommandType : byte
        {
            Add,
            Remove,
            ChangeStimulusMask,
            ChangeSecondaryChecksEnabled
        }

        private static readonly Queue<SensorsDatabaseCommandType> s_SensorsDatabaseCommandTypes = new Queue<SensorsDatabaseCommandType>();

        private static readonly Queue<AddSensorCommand> s_AddSensorCommands = new Queue<AddSensorCommand>();
        private static readonly Queue<RemoveSensorCommand> s_RemoveSensorCommands = new Queue<RemoveSensorCommand>();
        private static readonly Queue<ChangeSensorStimulusMaskCommand> s_ChangeSensorStimulusMaskCommands = new Queue<ChangeSensorStimulusMaskCommand>();
        private static readonly Queue<ChangeSensorSecondaryCheckEnabledCommand> s_ChangeSensorSecondaryCheckEnabledCommands = new Queue<ChangeSensorSecondaryCheckEnabledCommand>();

        private static void ResetSensorsDatabaseCommandBuffer()
        {
            s_SensorsDatabaseCommandTypes.Clear();

            s_AddSensorCommands.Clear();
            s_RemoveSensorCommands.Clear();
            s_ChangeSensorStimulusMaskCommands.Clear();
            s_ChangeSensorSecondaryCheckEnabledCommands.Clear();
        }

        internal static void AddSensor(HiraBotSensor sensor, int stimulusMask, bool lineOfSightCheck, bool navDistanceCheck)
        {
            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.Add);
            var secondaryChecksFlags =
                (lineOfSightCheck ? SensorSecondaryChecksFlags.LineOfSight : SensorSecondaryChecksFlags.None)
                | (navDistanceCheck ? SensorSecondaryChecksFlags.NavDistance : SensorSecondaryChecksFlags.None);
            s_AddSensorCommands.Enqueue(new AddSensorCommand(sensor, stimulusMask, secondaryChecksFlags));
        }

        internal static void RemoveSensor(HiraBotSensor sensor)
        {
            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.Remove);
            s_RemoveSensorCommands.Enqueue(new RemoveSensorCommand(sensor));
        }

        internal static void ChangeStimulusMask(HiraBotSensor sensor, int newStimulusMask)
        {
            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.ChangeStimulusMask);
            s_ChangeSensorStimulusMaskCommands.Enqueue(new ChangeSensorStimulusMaskCommand(sensor, newStimulusMask));
        }

        internal static void ChangeSecondaryChecksEnabled(HiraBotSensor sensor, bool lineOfSightCheck, bool navDistanceCheck)
        {
            s_SensorsDatabaseCommandTypes.Enqueue(SensorsDatabaseCommandType.ChangeSecondaryChecksEnabled);
            var secondaryChecksFlags =
                (lineOfSightCheck ? SensorSecondaryChecksFlags.LineOfSight : SensorSecondaryChecksFlags.None)
                | (navDistanceCheck ? SensorSecondaryChecksFlags.NavDistance : SensorSecondaryChecksFlags.None);
            s_ChangeSensorSecondaryCheckEnabledCommands.Enqueue(new ChangeSensorSecondaryCheckEnabledCommand(sensor, secondaryChecksFlags));
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

                            s_SensorsSecondaryChecks.Reallocate(s_SensorsCount + 8, Allocator.Persistent,
                                NativeArrayOptions.UninitializedMemory);

                            System.Array.Resize(ref s_Sensors, s_SensorsCount + 8);
                        }

                        s_SensorsStimulusMasks[s_SensorsCount] = cmd.stimulusMask;

                        s_SensorsSecondaryChecks[s_SensorsCount] = cmd.secondaryCheckFlags;

                        s_SensorsLookUpTable.Add(cmd.sensor, s_SensorsCount);

                        s_Sensors[s_SensorsCount] = cmd.sensor;

                        s_SensorsCount++;

                        InitializeSensor(cmd.sensor);

                        break;
                    }
                    case SensorsDatabaseCommandType.Remove:
                    {
                        var cmd = s_RemoveSensorCommands.Dequeue();
                        if (!s_SensorsLookUpTable.TryGetValue(cmd.sensor, out var indexToRemove))
                        {
                            break;
                        }

                        ShutdownSensor(cmd.sensor);

                        var lastSensorIndex = s_SensorsCount - 1;
                        var lastSensor = s_Sensors[lastSensorIndex];

                        s_SensorsStimulusMasks[indexToRemove] = s_SensorsStimulusMasks[lastSensorIndex];
                        s_SensorsSecondaryChecks[indexToRemove] = s_SensorsSecondaryChecks[lastSensorIndex];

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
                    case SensorsDatabaseCommandType.ChangeSecondaryChecksEnabled:
                    {
                        var cmd = s_ChangeSensorSecondaryCheckEnabledCommands.Dequeue();
                        if (!s_SensorsLookUpTable.TryGetValue(cmd.sensor, out var indexToChange))
                        {
                            break;
                        }

                        s_SensorsSecondaryChecks[indexToChange] = cmd.secondaryChecksFlags;

                        if (cmd.secondaryChecksFlags == SensorSecondaryChecksFlags.None)
                        {
                            if (!cmd.sensor.m_PerceivedObjectsLocations.IsCreated)
                            {
                                cmd.sensor.m_PerceivedObjectsLocations = UnmanagedCollections.CreateUnmanagedList<Unity.Mathematics.float4>(Allocator.Persistent);
                            }
                        }
                        else
                        {
                            if (cmd.sensor.m_PerceivedObjectsLocations.IsCreated)
                            {
                                cmd.sensor.m_PerceivedObjectsLocations.Dispose();
                                cmd.sensor.m_PerceivedObjectsLocations = default;
                            }
                        }

                        break;
                    }
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }

            if (s_SensorsDatabaseCommandTypes.Count == 0
                && s_SensorsDatabaseCommandTypes.Count == s_AddSensorCommands.Count
                && s_AddSensorCommands.Count == s_RemoveSensorCommands.Count
                && s_RemoveSensorCommands.Count == s_ChangeSensorStimulusMaskCommands.Count
                && s_ChangeSensorStimulusMaskCommands.Count == s_ChangeSensorSecondaryCheckEnabledCommands.Count)
            {
                return;
            }

#if HIRA_BOTS_CREATOR_MODE
            Debug.Break();
            Debug.LogError("Command buffers not correctly cleared in Perception System sensors database.");
#endif
        }

        private static void InitializeSensor(HiraBotSensor sensor)
        {
            sensor.m_UpdateJob = null;

            sensor.m_PerceivedObjects = UnmanagedCollections.CreateDictionary<int, float>(Allocator.Persistent);

            sensor.m_ObjectsPerceivedThisFrame = UnmanagedCollections.CreateUnmanagedList<int>(Allocator.Persistent);

            sensor.m_NewObjectsPerceived = UnmanagedCollections.CreateUnmanagedList<int>(Allocator.Persistent);

            sensor.m_ObjectsStoppedPerceiving = UnmanagedCollections.CreateUnmanagedList<int>(Allocator.Persistent);

            if (sensor.lineOfSightCheck.m_Enabled)// || sensor.navDistanceCheck.m_Enabled)
            {
                sensor.m_PerceivedObjectsLocations = UnmanagedCollections.CreateUnmanagedList<Unity.Mathematics.float4>(Allocator.Persistent);
            }
        }

        private static void ShutdownSensor(HiraBotSensor sensor)
        {
            if (sensor.m_PerceivedObjectsLocations.IsCreated)
            {
                sensor.m_PerceivedObjectsLocations.Dispose();
                sensor.m_PerceivedObjectsLocations = default;
            }

            sensor.m_ObjectsStoppedPerceiving.DisposeUnmanagedList();
            sensor.m_ObjectsStoppedPerceiving = default;

            sensor.m_NewObjectsPerceived.DisposeUnmanagedList();
            sensor.m_NewObjectsPerceived = default;

            sensor.m_ObjectsPerceivedThisFrame.DisposeUnmanagedList();
            sensor.m_ObjectsPerceivedThisFrame = default;

            sensor.m_PerceivedObjects.DisposeUnmanagedDictionary();
            sensor.m_PerceivedObjects = default;

            sensor.m_UpdateJob = null;
        }
    }
}