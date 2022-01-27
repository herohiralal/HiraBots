using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal static partial class PerceptionSystem
    {
        private struct AddStimulusCommand
        {
            internal AddStimulusCommand(ulong id, float3 position, int stimulusType, int associatedObject)
            {
                this.id = id;
                this.position = position;
                this.stimulusType = stimulusType;
                this.associatedObject = associatedObject;
            }

            internal ulong id { get; }
            internal float3 position { get; }
            internal int stimulusType { get; }
            internal int associatedObject { get; }
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
            internal ChangeStimulusTypeCommand(ulong id, int newStimulusType)
            {
                this.id = id;
                this.newStimulusType = newStimulusType;
            }

            internal ulong id { get; }
            internal int newStimulusType { get; }
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

        private static Queue<StimuliDatabaseCommandType> s_StimuliDatabaseCommandTypes;

        private static Queue<AddStimulusCommand> s_AddStimulusCommands;
        private static Queue<RemoveStimulusCommand> s_RemoveStimulusCommands;
        private static Queue<ChangeStimulusPositionCommand> s_ChangeStimulusPositionCommands;
        private static Queue<ChangeStimulusTypeCommand> s_ChangeStimulusTypeCommands;
        private static Queue<ChangeStimulusAssociatedObjectCommand> s_ChangeStimulusAssociatedObjectCommands;

        private static void InitializeStimuliDatabaseCommandBuffer()
        {
            s_StimuliDatabaseCommandTypes = new Queue<StimuliDatabaseCommandType>();

            s_AddStimulusCommands = new Queue<AddStimulusCommand>();
            s_RemoveStimulusCommands = new Queue<RemoveStimulusCommand>();
            s_ChangeStimulusPositionCommands = new Queue<ChangeStimulusPositionCommand>();
            s_ChangeStimulusTypeCommands = new Queue<ChangeStimulusTypeCommand>();
            s_ChangeStimulusAssociatedObjectCommands = new Queue<ChangeStimulusAssociatedObjectCommand>();
        }

        private static void ShutdownStimuliDatabaseCommandBuffer()
        {
            s_ChangeStimulusAssociatedObjectCommands = null;
            s_ChangeStimulusTypeCommands = null;
            s_ChangeStimulusPositionCommands = null;
            s_RemoveStimulusCommands = null;
            s_AddStimulusCommands = null;

            s_StimuliDatabaseCommandTypes = null;
        }

        internal static ulong AddStimulus(Vector3 position, int stimulusType, int associatedObject)
        {
            s_Id++;

            s_StimuliDatabaseCommandTypes.Enqueue(StimuliDatabaseCommandType.AddStimulus);
            s_AddStimulusCommands.Enqueue(new AddStimulusCommand(s_Id, position, stimulusType, associatedObject));

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

        internal static void ChangeStimulusType(ulong id, int newType)
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
                        break;
                    }
                    case StimuliDatabaseCommandType.RemoveStimulus:
                    {
                        break;
                    }
                    case StimuliDatabaseCommandType.ChangeStimulusPosition:
                    {
                        break;
                    }
                    case StimuliDatabaseCommandType.ChangeStimulusType:
                    {
                        break;
                    }
                    case StimuliDatabaseCommandType.ChangeStimulusAssociatedObject:
                    {
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
    }
}