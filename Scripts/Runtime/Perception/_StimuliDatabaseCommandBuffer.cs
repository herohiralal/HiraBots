﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal static partial class PerceptionSystem
    {
        private struct AddStimulusCommand
        {
            internal AddStimulusCommand(byte stimulusType, float3 position, int associatedObject, ulong id)
            {
                this.stimulusType = stimulusType;
                this.position = position;
                this.associatedObject = associatedObject;
                this.id = id;
            }

            internal byte stimulusType { get; }
            internal float3 position { get; }
            internal int associatedObject { get; }
            internal ulong id { get; }
        }

        private struct RemoveStimulusCommand
        {
            internal RemoveStimulusCommand(ulong id)
            {
                this.id = id;
            }

            internal ulong id { get; }
        }

        private struct ChangeStimulusPositionCommand
        {
            internal ChangeStimulusPositionCommand(ulong id, float3 newPos)
            {
                this.id = id;
                this.newPos = newPos;
            }

            internal ulong id { get; }
            internal float3 newPos { get; }
        }

        private struct ChangeStimulusTypeCommand
        {
            internal ChangeStimulusTypeCommand(ulong id, byte newStimulusType)
            {
                this.id = id;
                this.newStimulusType = newStimulusType;
            }

            internal ulong id { get; }
            internal byte newStimulusType { get; }
        }

        private struct ChangeStimulusAssociatedObjectCommand
        {
            internal ChangeStimulusAssociatedObjectCommand(ulong id, int newAssociatedObject)
            {
                this.id = id;
                this.newAssociatedObject = newAssociatedObject;
            }

            internal ulong id { get; }
            internal int newAssociatedObject { get; }
        }

        private enum StimuliDatabaseCommandType : byte
        {
            AddStimulus,
            RemoveStimulus,
            ChangeStimulusPosition,
            ChangeStimulusType,
            ChangeStimulusAssociatedObject
        }

        private static readonly Queue<StimuliDatabaseCommandType> s_StimuliDatabaseCommandTypes
            = new Queue<StimuliDatabaseCommandType>();

        private static readonly Queue<AddStimulusCommand> s_AddStimulusCommands
            = new Queue<AddStimulusCommand>();

        private static readonly Queue<RemoveStimulusCommand> s_RemoveStimulusCommands
            = new Queue<RemoveStimulusCommand>();

        private static readonly Queue<ChangeStimulusPositionCommand> s_ChangeStimulusPositionCommands
            = new Queue<ChangeStimulusPositionCommand>();

        private static readonly Queue<ChangeStimulusTypeCommand> s_ChangeStimulusTypeCommands
            = new Queue<ChangeStimulusTypeCommand>();

        private static readonly Queue<ChangeStimulusAssociatedObjectCommand> s_ChangeStimulusAssociatedObjectCommands
            = new Queue<ChangeStimulusAssociatedObjectCommand>();

        private static void ResetStimuliDatabaseCommandBuffer()
        {
            s_StimuliDatabaseCommandTypes.Clear();

            s_AddStimulusCommands.Clear();
            s_RemoveStimulusCommands.Clear();
            s_ChangeStimulusPositionCommands.Clear();
            s_ChangeStimulusTypeCommands.Clear();
            s_ChangeStimulusAssociatedObjectCommands.Clear();
        }

        internal static ulong AddStimulus(byte stimulusType, Vector3 position, int associatedObject)
        {
            s_Id++;

            s_StimuliDatabaseCommandTypes.Enqueue(StimuliDatabaseCommandType.AddStimulus);
            s_AddStimulusCommands.Enqueue(new AddStimulusCommand(stimulusType, position, associatedObject, s_Id));

            return s_Id;
        }

        internal static void RemoveStimulus(ulong id)
        {
            s_StimuliDatabaseCommandTypes.Enqueue(StimuliDatabaseCommandType.RemoveStimulus);
            s_RemoveStimulusCommands.Enqueue(new RemoveStimulusCommand(id));
        }

        internal static void ChangeStimulusPosition(ulong id, Vector3 newPos)
        {
            s_StimuliDatabaseCommandTypes.Enqueue(StimuliDatabaseCommandType.ChangeStimulusPosition);
            s_ChangeStimulusPositionCommands.Enqueue(new ChangeStimulusPositionCommand(id, newPos));
        }

        internal static void ChangeStimulusType(ulong id, byte newType)
        {
            s_StimuliDatabaseCommandTypes.Enqueue(StimuliDatabaseCommandType.ChangeStimulusType);
            s_ChangeStimulusTypeCommands.Enqueue(new ChangeStimulusTypeCommand(id, newType));
        }

        internal static void ChangeStimulusAssociatedObject(ulong id, int newAssociatedObject)
        {
            s_StimuliDatabaseCommandTypes.Enqueue(StimuliDatabaseCommandType.ChangeStimulusAssociatedObject);
            s_ChangeStimulusAssociatedObjectCommands.Enqueue(new ChangeStimulusAssociatedObjectCommand(id, newAssociatedObject));
        }

        private static void ApplyStimuliDatabaseCommandBuffer()
        {
            while (s_StimuliDatabaseCommandTypes.Count > 0)
            {
                switch (s_StimuliDatabaseCommandTypes.Dequeue())
                {
                    case StimuliDatabaseCommandType.AddStimulus:
                    {
                        var cmd = s_AddStimulusCommands.Dequeue();
                        if (s_StimuliLookUpTable.ContainsKey(cmd.id))
                        {
                            break;
                        }

                        AddStimulus(cmd.stimulusType, new float4(cmd.position, 1), cmd.associatedObject, cmd.id);
                        break;
                    }
                    case StimuliDatabaseCommandType.RemoveStimulus:
                    {
                        var cmd = s_RemoveStimulusCommands.Dequeue();
                        if (!s_StimuliLookUpTable.TryGetValue(cmd.id, out var toRemove))
                        {
                            break;
                        }

                        RemoveStimulusAt(toRemove.type, toRemove.index);
                        break;
                    }
                    case StimuliDatabaseCommandType.ChangeStimulusPosition:
                    {
                        var cmd = s_ChangeStimulusPositionCommands.Dequeue();
                        if (!s_StimuliLookUpTable.TryGetValue(cmd.id, out var i))
                        {
                            break;
                        }

                        s_StimuliPositions[i.type][i.index] = new float4(cmd.newPos, 1);
                        break;
                    }
                    case StimuliDatabaseCommandType.ChangeStimulusType:
                    {
                        var cmd = s_ChangeStimulusTypeCommands.Dequeue();
                        if (!s_StimuliLookUpTable.TryGetValue(cmd.id, out var i))
                        {
                            break;
                        }

                        // don't inline these, they're gonna get overwritten when removing previous
                        var pos = s_StimuliPositions[i.type][i.index];
                        var asObj = s_StimuliAssociatedObjects[i.type][i.index];
                        var id = s_Stimuli[i.type][i.index];

                        RemoveStimulusAt(i.type, i.index);
                        AddStimulus(cmd.newStimulusType, pos, asObj, id);

                        break;
                    }
                    case StimuliDatabaseCommandType.ChangeStimulusAssociatedObject:
                    {
                        var cmd = s_ChangeStimulusAssociatedObjectCommands.Dequeue();
                        if (!s_StimuliLookUpTable.TryGetValue(cmd.id, out var i))
                        {
                            break;
                        }

                        s_StimuliAssociatedObjects[i.type][i.index] = cmd.newAssociatedObject;
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (s_StimuliDatabaseCommandTypes.Count == 0
                && s_StimuliDatabaseCommandTypes.Count == s_AddStimulusCommands.Count
                && s_AddStimulusCommands.Count == s_RemoveStimulusCommands.Count
                && s_RemoveStimulusCommands.Count == s_ChangeStimulusPositionCommands.Count
                && s_ChangeStimulusPositionCommands.Count == s_ChangeStimulusTypeCommands.Count
                && s_ChangeStimulusTypeCommands.Count == s_ChangeStimulusAssociatedObjectCommands.Count)
            {
                return;
            }

#if HIRA_BOTS_CREATOR_MODE
            Debug.Break();
            Debug.LogError("Command buffers not correctly cleared in Perception System stimuli database.");
#endif
        }

        private static void AddStimulus(byte stimulusType, float4 position, int associatedObject, ulong id)
        {
            var currentStimuliCount = s_StimuliCounts[stimulusType];

            if (currentStimuliCount == s_Stimuli[stimulusType].Length)
            {
                // reallocation time
                s_StimuliPositions[stimulusType]
                    .Reallocate(currentStimuliCount + 8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

                s_StimuliAssociatedObjects[stimulusType]
                    .Reallocate(currentStimuliCount + 8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

                s_Stimuli[stimulusType]
                    .Reallocate(currentStimuliCount + 8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            }

            s_StimuliPositions[stimulusType][currentStimuliCount] = position;

            s_StimuliAssociatedObjects[stimulusType][currentStimuliCount] = associatedObject;

            s_StimuliLookUpTable.Add(id, (stimulusType, currentStimuliCount));

            s_Stimuli[stimulusType][currentStimuliCount] = id;

            s_StimuliCounts[stimulusType]++;
        }

        private static void RemoveStimulusAt(byte type, int index)
        {
            var lastStimulusIndex = s_StimuliCounts[type] - 1;
            var lastStimulus = s_Stimuli[type][lastStimulusIndex];

            s_StimuliPositions[type][index] = s_StimuliPositions[type][lastStimulusIndex];

            s_StimuliAssociatedObjects[type][index] = s_StimuliAssociatedObjects[type][lastStimulusIndex];

            s_StimuliLookUpTable[lastStimulus] = (type, index);
            s_StimuliLookUpTable.Remove(s_Stimuli[type][index]);

            s_Stimuli[type][index] = lastStimulus;

            s_StimuliCounts[type]--;
        }
    }
}