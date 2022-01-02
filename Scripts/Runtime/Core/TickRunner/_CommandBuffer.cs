using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    internal partial class HiraBotsModule
    {
        private struct AddServiceCommand
        {
            internal AddServiceCommand(IHiraBotsService obj, float tickInterval, float timeDilation)
            {
                this.obj = obj;
                this.tickInterval = tickInterval;
                this.timeDilation = timeDilation;
            }

            internal IHiraBotsService obj { get; }
            internal float tickInterval { get; }
            internal float timeDilation { get; }
        }

        private struct RemoveServiceCommand
        {
            internal RemoveServiceCommand(IHiraBotsService obj)
            {
                this.obj = obj;
            }

            internal IHiraBotsService obj { get; }
        }

        private struct ChangeServiceTimeDilationCommand
        {
            internal ChangeServiceTimeDilationCommand(IHiraBotsService obj, float timeDilation)
            {
                this.obj = obj;
                this.timeDilation = timeDilation;
            }

            internal IHiraBotsService obj { get; }
            internal float timeDilation { get; }
        }

        private struct AddTaskCommand
        {
            internal AddTaskCommand(ExecutorComponent obj, IHiraBotsTask task, float tickInterval, float timeDilation)
            {
                this.obj = obj;
                this.task = task;
                this.tickInterval = tickInterval;
                this.timeDilation = timeDilation;
            }

            internal ExecutorComponent obj { get; }
            internal IHiraBotsTask task { get; }
            internal float tickInterval { get; }
            internal float timeDilation { get; }
        }

        private struct RemoveTaskCommand
        {
            internal RemoveTaskCommand(ExecutorComponent obj)
            {
                this.obj = obj;
            }

            internal ExecutorComponent obj { get; }
        }

        private struct ChangeTaskTimeDilationCommand
        {
            internal ChangeTaskTimeDilationCommand(ExecutorComponent obj, float timeDilation)
            {
                this.obj = obj;
                this.timeDilation = timeDilation;
            }

            internal ExecutorComponent obj { get; }
            internal float timeDilation { get; }
        }

        private enum CommandType : byte
        {
            AddService,
            RemoveService,
            ChangeServiceTimeDilation,
            AddTask,
            RemoveTask,
            ChangeTaskTimeDilation
        }

        private Queue<CommandType> m_CommandTypes;
        private Queue<AddServiceCommand> m_AddServiceCommands;
        private Queue<RemoveServiceCommand> m_RemoveServiceCommands;
        private Queue<ChangeServiceTimeDilationCommand> m_ChangeServiceTimeDilationCommands;
        private Queue<AddTaskCommand> m_AddTaskCommands;
        private Queue<RemoveTaskCommand> m_RemoveTaskCommands;
        private Queue<ChangeTaskTimeDilationCommand> m_ChangeTaskTimeDilationCommands;

        private void InitializeCommandBuffer()
        {
            m_CommandTypes = new Queue<CommandType>();
            m_AddServiceCommands = new Queue<AddServiceCommand>();
            m_RemoveServiceCommands = new Queue<RemoveServiceCommand>();
            m_ChangeServiceTimeDilationCommands = new Queue<ChangeServiceTimeDilationCommand>();
            m_AddTaskCommands = new Queue<AddTaskCommand>();
            m_RemoveTaskCommands = new Queue<RemoveTaskCommand>();
            m_ChangeTaskTimeDilationCommands = new Queue<ChangeTaskTimeDilationCommand>();
        }

        private void ShutdownCommandBuffer()
        {
            m_ChangeTaskTimeDilationCommands = null;
            m_RemoveTaskCommands = null;
            m_AddTaskCommands = null;
            m_ChangeServiceTimeDilationCommands = null;
            m_RemoveServiceCommands = null;
            m_AddServiceCommands = null;
            m_CommandTypes = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferAddServiceCommand(IHiraBotsService service, float tickInterval, float timeDilation)
        {
            m_CommandTypes.Enqueue(CommandType.AddService);
            m_AddServiceCommands.Enqueue(new AddServiceCommand(service, tickInterval, timeDilation));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferRemoveServiceCommand(IHiraBotsService service)
        {
            m_CommandTypes.Enqueue(CommandType.RemoveService);
            m_RemoveServiceCommands.Enqueue(new RemoveServiceCommand(service));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeServiceTimeDilationCommand(IHiraBotsService service, float timeDilation)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeServiceTimeDilation);
            m_ChangeServiceTimeDilationCommands.Enqueue(new ChangeServiceTimeDilationCommand(service, timeDilation));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferAddTaskCommand(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float timeDilation)
        {
            m_CommandTypes.Enqueue(CommandType.AddTask);
            m_AddTaskCommands.Enqueue(new AddTaskCommand(executor, task, tickInterval, timeDilation));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferRemoveTaskCommand(ExecutorComponent executor)
        {
            m_CommandTypes.Enqueue(CommandType.RemoveTask);
            m_RemoveTaskCommands.Enqueue(new RemoveTaskCommand(executor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BufferChangeTaskTimeDilationCommand(ExecutorComponent executor, float timeDilation)
        {
            m_CommandTypes.Enqueue(CommandType.ChangeTaskTimeDilation);
            m_ChangeTaskTimeDilationCommands.Enqueue(new ChangeTaskTimeDilationCommand(executor, timeDilation));
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
                        AddServiceInternal(cmd.obj, cmd.tickInterval, cmd.timeDilation);
                        break;
                    }
                    case CommandType.RemoveService:
                    {
                        var cmd = m_RemoveServiceCommands.Dequeue();
                        RemoveServiceInternal(cmd.obj);
                        break;
                    }
                    case CommandType.ChangeServiceTimeDilation:
                    {
                        var cmd = m_ChangeServiceTimeDilationCommands.Dequeue();
                        ChangeServiceTimeDilationInternal(cmd.obj, cmd.timeDilation);
                        break;
                    }
                    case CommandType.AddTask:
                    {
                        var cmd = m_AddTaskCommands.Dequeue();
                        AddTaskInternal(cmd.obj, cmd.task, cmd.tickInterval, cmd.timeDilation);
                        break;
                    }
                    case CommandType.RemoveTask:
                    {
                        var cmd = m_RemoveTaskCommands.Dequeue();
                        RemoveTaskInternal(cmd.obj);
                        break;
                    }
                    case CommandType.ChangeTaskTimeDilation:
                    {
                        var cmd = m_ChangeTaskTimeDilationCommands.Dequeue();
                        ChangeTaskTimeDilationInternal(cmd.obj, cmd.timeDilation);
                        break;
                    }
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }

                if (m_CommandTypes.Count == 0
                    && m_CommandTypes.Count == m_AddServiceCommands.Count
                    && m_AddServiceCommands.Count == m_RemoveServiceCommands.Count
                    && m_RemoveServiceCommands.Count == m_ChangeServiceTimeDilationCommands.Count
                    && m_ChangeServiceTimeDilationCommands.Count == m_AddTaskCommands.Count
                    && m_AddTaskCommands.Count == m_RemoveTaskCommands.Count
                    && m_RemoveTaskCommands.Count == m_ChangeTaskTimeDilationCommands.Count)
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