using System.Collections.Generic;
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

        private struct RemoveCommand<T>
        {
            internal RemoveCommand(T obj)
            {
                this.obj = obj;
            }

            internal T obj { get; }
        }

        private struct ChangeTickIntervalMultiplierCommand<T>
        {
            internal ChangeTickIntervalMultiplierCommand(T obj, float multiplier)
            {
                this.obj = obj;
                this.multiplier = multiplier;
            }

            internal T obj { get; }
            internal float multiplier { get; }
        }

        private struct ChangeTickPausedCommand<T>
        {
            internal ChangeTickPausedCommand(T obj, bool value)
            {
                this.obj = obj;
                this.value = value;
            }

            internal T obj { get; }
            internal bool value { get; }
        }

        private enum CommandType : byte
        {
            AddService,
            RemoveService,
            ChangeServiceTickIntervalMultiplier,
            ChangeServiceTickPaused,
            AddTask,
            RemoveTask,
            ChangeTaskTickIntervalMultiplier,
            ChangeTaskTickPaused
        }

        private Queue<CommandType> m_CommandTypes;
        private Queue<AddServiceCommand> m_AddServiceCommands;
        private Queue<RemoveCommand<IHiraBotsService>> m_RemoveServiceCommands;
        private Queue<ChangeTickIntervalMultiplierCommand<IHiraBotsService>> m_ChangeServiceTickIntervalMultiplierCommands;
        private Queue<ChangeTickPausedCommand<IHiraBotsService>> m_ChangeServiceTickPausedCommands;
        private Queue<AddTaskCommand> m_AddTaskCommands;
        private Queue<RemoveCommand<ExecutorComponent>> m_RemoveTaskCommands;
        private Queue<ChangeTickIntervalMultiplierCommand<ExecutorComponent>> m_ChangeTaskTickIntervalMultiplierCommands;
        private Queue<ChangeTickPausedCommand<ExecutorComponent>> m_ChangeTaskTickPausedCommands;

        private void InitializeCommandBuffer()
        {
            m_CommandTypes = new Queue<CommandType>();
            m_AddServiceCommands = new Queue<AddServiceCommand>();
            m_RemoveServiceCommands = new Queue<RemoveCommand<IHiraBotsService>>();
            m_ChangeServiceTickIntervalMultiplierCommands = new Queue<ChangeTickIntervalMultiplierCommand<IHiraBotsService>>();
            m_ChangeServiceTickPausedCommands = new Queue<ChangeTickPausedCommand<IHiraBotsService>>();
            m_AddTaskCommands = new Queue<AddTaskCommand>();
            m_RemoveTaskCommands = new Queue<RemoveCommand<ExecutorComponent>>();
            m_ChangeTaskTickIntervalMultiplierCommands = new Queue<ChangeTickIntervalMultiplierCommand<ExecutorComponent>>();
            m_ChangeTaskTickPausedCommands = new Queue<ChangeTickPausedCommand<ExecutorComponent>>();
        }

        private void ShutdownCommandBuffer()
        {
            m_ChangeTaskTickPausedCommands = null;
            m_ChangeTaskTickIntervalMultiplierCommands = null;
            m_RemoveTaskCommands = null;
            m_AddTaskCommands = null;
            m_ChangeServiceTickPausedCommands = null;
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
            m_RemoveServiceCommands.Enqueue(new RemoveCommand<IHiraBotsService>(service));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeServiceTickIntervalMultiplierCommand(IHiraBotsService service, float tickIntervalMultiplier)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeServiceTickIntervalMultiplier);
            m_ChangeServiceTickIntervalMultiplierCommands.Enqueue(new ChangeTickIntervalMultiplierCommand<IHiraBotsService>(service, tickIntervalMultiplier));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeServiceTickPausedCommand(IHiraBotsService service, bool value)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeServiceTickPaused);
            m_ChangeServiceTickPausedCommands.Enqueue(new ChangeTickPausedCommand<IHiraBotsService>(service, value));
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
            m_RemoveTaskCommands.Enqueue(new RemoveCommand<ExecutorComponent>(executor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeTaskTickIntervalMultiplierCommand(ExecutorComponent executor, float tickIntervalMultiplier)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeTaskTickIntervalMultiplier);
            m_ChangeTaskTickIntervalMultiplierCommands.Enqueue(new ChangeTickIntervalMultiplierCommand<ExecutorComponent>(executor, tickIntervalMultiplier));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeTaskTickPausedCommand(ExecutorComponent executor, bool value)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeTaskTickPaused);
            m_ChangeTaskTickPausedCommands.Enqueue(new ChangeTickPausedCommand<ExecutorComponent>(executor, value));
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
                    case CommandType.ChangeServiceTickIntervalMultiplier:
                    {
                        var cmd = m_ChangeServiceTickIntervalMultiplierCommands.Dequeue();
                        ChangeServiceTickIntervalMultiplierInternal(cmd.obj, cmd.multiplier);
                        break;
                    }
                    case CommandType.ChangeServiceTickPaused:
                    {
                        var cmd = m_ChangeServiceTickPausedCommands.Dequeue();
                        ChangeServiceTickPausedInternal(cmd.obj, cmd.value);
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
                    case CommandType.ChangeTaskTickIntervalMultiplier:
                    {
                        var cmd = m_ChangeTaskTickIntervalMultiplierCommands.Dequeue();
                        ChangeTaskTickIntervalMultiplierInternal(cmd.obj, cmd.multiplier);
                        break;
                    }
                    case CommandType.ChangeTaskTickPaused:
                    {
                        var cmd = m_ChangeTaskTickPausedCommands.Dequeue();
                        ChangeTaskTickPausedInternal(cmd.obj, cmd.value);
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