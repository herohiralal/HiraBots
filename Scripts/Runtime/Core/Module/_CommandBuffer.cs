﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    internal partial class HiraBotsModule
    {
        private struct AddServiceCommand
        {
            internal AddServiceCommand(IHiraBotsService obj, float tickInterval, float tickIntervalMultiplier)
            {
                this.obj = obj;
                this.tickInterval = tickInterval;
                this.tickIntervalMultiplier = tickIntervalMultiplier;
            }

            internal IHiraBotsService obj { get; }
            internal float tickInterval { get; }
            internal float tickIntervalMultiplier { get; }
        }

        private struct RemoveServiceCommand
        {
            internal RemoveServiceCommand(IHiraBotsService obj)
            {
                this.obj = obj;
            }

            internal IHiraBotsService obj { get; }
        }

        private struct ChangeServiceTickIntervalMultiplierCommand
        {
            internal ChangeServiceTickIntervalMultiplierCommand(IHiraBotsService obj, float multiplier)
            {
                this.obj = obj;
                this.multiplier = multiplier;
            }

            internal IHiraBotsService obj { get; }
            internal float multiplier { get; }
        }

        private struct AddTaskCommand
        {
            internal AddTaskCommand(ExecutorComponent obj, IHiraBotsTask task, float tickInterval, float tickIntervalMultiplier)
            {
                this.obj = obj;
                this.task = task;
                this.tickInterval = tickInterval;
                this.tickIntervalMultiplier = tickIntervalMultiplier;
            }

            internal ExecutorComponent obj { get; }
            internal IHiraBotsTask task { get; }
            internal float tickInterval { get; }
            internal float tickIntervalMultiplier { get; }
        }

        private struct RemoveTaskCommand
        {
            internal RemoveTaskCommand(ExecutorComponent obj)
            {
                this.obj = obj;
            }

            internal ExecutorComponent obj { get; }
        }

        private struct ChangeTaskTickIntervalMultiplierCommand
        {
            internal ChangeTaskTickIntervalMultiplierCommand(ExecutorComponent obj, float multiplier)
            {
                this.obj = obj;
                this.multiplier = multiplier;
            }

            internal ExecutorComponent obj { get; }
            internal float multiplier { get; }
        }

        private enum CommandType : byte
        {
            AddService,
            RemoveService,
            ChangeServiceTickIntervalMultiplier,
            AddTask,
            RemoveTask,
            ChangeTaskTickIntervalMultiplier
        }

        private Queue<CommandType> m_CommandTypes;
        private Queue<AddServiceCommand> m_AddServiceCommands;
        private Queue<RemoveServiceCommand> m_RemoveServiceCommands;
        private Queue<ChangeServiceTickIntervalMultiplierCommand> m_ChangeServiceTickIntervalMultiplierCommands;
        private Queue<AddTaskCommand> m_AddTaskCommands;
        private Queue<RemoveTaskCommand> m_RemoveTaskCommands;
        private Queue<ChangeTaskTickIntervalMultiplierCommand> m_ChangeTaskTickIntervalMultiplierCommands;

        private void InitializeCommandBuffer()
        {
            m_CommandTypes = new Queue<CommandType>();
            m_AddServiceCommands = new Queue<AddServiceCommand>();
            m_RemoveServiceCommands = new Queue<RemoveServiceCommand>();
            m_ChangeServiceTickIntervalMultiplierCommands = new Queue<ChangeServiceTickIntervalMultiplierCommand>();
            m_AddTaskCommands = new Queue<AddTaskCommand>();
            m_RemoveTaskCommands = new Queue<RemoveTaskCommand>();
            m_ChangeTaskTickIntervalMultiplierCommands = new Queue<ChangeTaskTickIntervalMultiplierCommand>();
        }

        private void ShutdownCommandBuffer()
        {
            m_ChangeTaskTickIntervalMultiplierCommands = null;
            m_RemoveTaskCommands = null;
            m_AddTaskCommands = null;
            m_ChangeServiceTickIntervalMultiplierCommands = null;
            m_RemoveServiceCommands = null;
            m_AddServiceCommands = null;
            m_CommandTypes = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferAddServiceCommand(IHiraBotsService service, float tickInterval, float tickIntervalMultiplier)
        {
            m_CommandTypes.Enqueue(CommandType.AddService);
            m_AddServiceCommands.Enqueue(new AddServiceCommand(service, tickInterval, tickIntervalMultiplier));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferRemoveServiceCommand(IHiraBotsService service)
        {
            m_CommandTypes.Enqueue(CommandType.RemoveService);
            m_RemoveServiceCommands.Enqueue(new RemoveServiceCommand(service));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeServiceTickIntervalMultiplierCommand(IHiraBotsService service, float tickIntervalMultiplier)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeServiceTickIntervalMultiplier);
            m_ChangeServiceTickIntervalMultiplierCommands.Enqueue(new ChangeServiceTickIntervalMultiplierCommand(service, tickIntervalMultiplier));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferAddTaskCommand(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float tickIntervalMultiplier)
        {
            m_CommandTypes.Enqueue(CommandType.AddTask);
            m_AddTaskCommands.Enqueue(new AddTaskCommand(executor, task, tickInterval, tickIntervalMultiplier));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferRemoveTaskCommand(ExecutorComponent executor)
        {
            m_CommandTypes.Enqueue(CommandType.RemoveTask);
            m_RemoveTaskCommands.Enqueue(new RemoveTaskCommand(executor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeTaskTickIntervalMultiplierCommand(ExecutorComponent executor, float tickIntervalMultiplier)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeTaskTickIntervalMultiplier);
            m_ChangeTaskTickIntervalMultiplierCommands.Enqueue(new ChangeTaskTickIntervalMultiplierCommand(executor, tickIntervalMultiplier));
        }

        private void ApplyCommandBuffer()
        {
            while (m_CommandTypes.Count > 0)
            {
                var commandType = m_CommandTypes.Dequeue();

                switch (commandType)
                {
                    case CommandType.AddService:
                    {
                        var cmd = m_AddServiceCommands.Dequeue();
                        AddServiceInternal(cmd.obj, cmd.tickInterval, cmd.tickIntervalMultiplier);
                        break;
                    }
                    case CommandType.RemoveService:
                    {
                        var cmd = m_RemoveServiceCommands.Dequeue();
                        RemoveServiceInternal(cmd.obj);
                        break;
                    }
                    case CommandType.ChangeTaskTickIntervalMultiplier:
                    {
                        var cmd = m_ChangeTaskTickIntervalMultiplierCommands.Dequeue();
                        ChangeTaskTickIntervalMultiplierInternal(cmd.obj, cmd.multiplier);
                        break;
                    }
                    case CommandType.AddTask:
                    {
                        var cmd = m_AddTaskCommands.Dequeue();
                        AddTaskInternal(cmd.obj, cmd.task, cmd.tickInterval, cmd.tickIntervalMultiplier);
                        break;
                    }
                    case CommandType.RemoveTask:
                    {
                        var cmd = m_RemoveTaskCommands.Dequeue();
                        RemoveTaskInternal(cmd.obj);
                        break;
                    }
                    case CommandType.ChangeServiceTickIntervalMultiplier:
                    {
                        var cmd = m_ChangeServiceTickIntervalMultiplierCommands.Dequeue();
                        ChangeServiceTickIntervalMultiplierInternal(cmd.obj, cmd.multiplier);
                        break;
                    }
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }

                if (m_CommandTypes.Count == 0
                    && m_CommandTypes.Count == m_AddServiceCommands.Count
                    && m_AddServiceCommands.Count == m_RemoveServiceCommands.Count
                    && m_RemoveServiceCommands.Count == m_AddTaskCommands.Count
                    && m_AddTaskCommands.Count == m_RemoveTaskCommands.Count)
                {
                    return;
                }

#if HIRA_BOTS_CREATOR_MODE
                Debug.Break();
                Debug.LogError("Command buffers not correctly cleared in HiraBotsTickRunner.");
#endif
            }
        }
    }
}