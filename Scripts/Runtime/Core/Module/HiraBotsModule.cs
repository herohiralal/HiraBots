using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace HiraBots
{
    [DefaultExecutionOrder(1000)]
    [AddComponentMenu("")]
    internal partial class HiraBotsModule : MonoBehaviour, TaskRunner.IInterface, ServiceRunner.IInterface
    {
        private UpdateSystem<IHiraBotsService> m_ServiceUpdates;
        private UpdateSystem<ExecutorComponent> m_TaskUpdates;

        private JobHandle? m_UpdateJob;

        private static HiraBotsModule s_Instance;

        private void Awake()
        {
            // singleton stuff
            if (s_Instance != null)
            {
                Destroy(this);
                return;
            }

            s_Instance = this;
            CoroutineRunner.instance = this;
            TaskRunner.instance = this;
            ServiceRunner.instance = this;

            InitializeCommandBuffer();

            m_ServiceUpdates = new UpdateSystem<IHiraBotsService>(1);
            m_TaskUpdates = new UpdateSystem<ExecutorComponent>(1);

            m_UpdateJob = null;
        }

        private void OnDestroy()
        {
            // singleton stuff
            if (s_Instance != this)
            {
                return;
            }

            m_UpdateJob?.Complete();
            m_UpdateJob = null;

            ApplyCommandBuffer();

            ShutdownCommandBuffer();

            for (var i = 0; i < m_TaskUpdates.m_ObjectsCount; i++)
            {
                m_TaskUpdates.m_ObjectsBuffer[i].AbortTask();
            }

            m_TaskUpdates.Dispose();

            for (var i = 0; i < m_ServiceUpdates.m_ObjectsCount; i++)
            {
                m_ServiceUpdates.m_ObjectsBuffer[i].WrappedStop();
            }

            m_ServiceUpdates.Dispose();

            ServiceRunner.instance = this;
            TaskRunner.instance = this;
            CoroutineRunner.instance = null;
            s_Instance = null;
        }

        private void Update()
        {
            TickTasksUpdateSystem();
            TickServicesUpdateSystem();

            var deltaTime = Time.deltaTime;

            m_UpdateJob = JobHandle.CombineDependencies
            (
                m_TaskUpdates.ScheduleTickJob(deltaTime),
                m_ServiceUpdates.ScheduleTickJob(deltaTime)
            );
        }

        private void LateUpdate()
        {
            m_UpdateJob?.Complete();
            m_UpdateJob = null;

            ApplyCommandBuffer();
        }

        private unsafe void TickTasksUpdateSystem()
        {
            var shouldTicks = (float*) m_TaskUpdates.m_ShouldTick.GetUnsafeReadOnlyPtr();

            // iterate backwards so we can remove executors as we go, if not needed
            for (var i = m_TaskUpdates.m_ObjectsCount - 1; i >= 0; i--)
            {
                var deltaTime = shouldTicks[i];

                if (deltaTime < 0f)
                {
                    continue;
                }

                // execute
                var currentExecutor = m_TaskUpdates.m_ObjectsBuffer[i];
                var keepExecuting = currentExecutor.Tick(deltaTime);

                // remove if not needed anymore
                if (!keepExecuting)
                {
                    m_TaskUpdates.Remove(i);
                }
            }
        }

        private unsafe void TickServicesUpdateSystem()
        {
            var shouldTicks = (float*) m_ServiceUpdates.m_ShouldTick.GetUnsafeReadOnlyPtr();

            for (var i = m_ServiceUpdates.m_ObjectsCount - 1; i >= 0; i--)
            {
                var deltaTime = shouldTicks[i];

                if (deltaTime <= 0f)
                {
                    continue;
                }

                m_ServiceUpdates.m_ObjectsBuffer[i].WrappedTick(deltaTime);
            }
        }

        void ServiceRunner.IInterface.Add(IHiraBotsService service, float tickInterval, float tickIntervalMultiplier)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferAddServiceCommand(service, tickInterval, tickIntervalMultiplier);
            }
            else
            {
                AddServiceInternal(service, tickInterval, tickIntervalMultiplier);
            }
        }

        void ServiceRunner.IInterface.Remove(IHiraBotsService service)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferRemoveServiceCommand(service);
            }
            else
            {
                RemoveServiceInternal(service);
            }
        }

        void ServiceRunner.IInterface.ChangeTickIntervalMultiplier(IHiraBotsService service, float tickIntervalMultiplier)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferChangeServiceTickIntervalMultiplierCommand(service, tickIntervalMultiplier);
            }
            else
            {
                ChangeServiceTickIntervalMultiplierInternal(service, tickIntervalMultiplier);
            }
        }

        void ServiceRunner.IInterface.ChangeTickPaused(IHiraBotsService service, bool value)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferChangeServiceTickPausedCommand(service, value);
            }
            else
            {
                ChangeServiceTickPausedInternal(service, value);
            }
        }

        void TaskRunner.IInterface.Add(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float tickIntervalMultiplier)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferAddTaskCommand(executor, task, tickInterval, tickIntervalMultiplier);
            }
            else
            {
                AddTaskInternal(executor, task, tickInterval, tickIntervalMultiplier);
            }
        }

        void TaskRunner.IInterface.Remove(ExecutorComponent executor)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferRemoveTaskCommand(executor);
            }
            else
            {
                RemoveTaskInternal(executor);
            }
        }

        void TaskRunner.IInterface.ChangeTickIntervalMultiplier(ExecutorComponent executor, float tickIntervalMultiplier)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferChangeTaskTickIntervalMultiplierCommand(executor, tickIntervalMultiplier);
            }
            else
            {
                ChangeTaskTickIntervalMultiplierInternal(executor, tickIntervalMultiplier);
            }
        }

        void TaskRunner.IInterface.ChangeTickPaused(ExecutorComponent executor, bool value)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferChangeTaskTickPausedCommand(executor, value);
            }
            else
            {
                ChangeTaskTickPausedInternal(executor, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddServiceInternal(IHiraBotsService service, float tickInterval, float tickIntervalMultiplier)
        {
            if (m_ServiceUpdates.Add(service, tickInterval, tickIntervalMultiplier))
            {
                service.WrappedStart();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveServiceInternal(IHiraBotsService service)
        {
            if (m_ServiceUpdates.Remove(service))
            {
                service.WrappedStop();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeServiceTickIntervalMultiplierInternal(IHiraBotsService service, float tickIntervalMultiplier)
        {
            if (m_ServiceUpdates.m_IndexLookUp.TryGetValue(service, out var index))
            {
                m_ServiceUpdates.m_TickIntervalMultipliers[index] = tickIntervalMultiplier;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeServiceTickPausedInternal(IHiraBotsService service, bool newValue)
        {
            if (m_ServiceUpdates.m_IndexLookUp.TryGetValue(service, out var index))
            {
                m_ServiceUpdates.m_TickPaused[index] = newValue;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddTaskInternal(ExecutorComponent executor, IHiraBotsTask task, float tickInterval, float tickIntervalMultiplier)
        {
            if (m_TaskUpdates.Add(executor, tickInterval, tickIntervalMultiplier))
            {
                executor.BeginTask(task);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveTaskInternal(ExecutorComponent executor)
        {
            if (m_TaskUpdates.Remove(executor))
            {
                executor.AbortTask();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeTaskTickIntervalMultiplierInternal(ExecutorComponent executor, float tickIntervalMultiplier)
        {
            if (m_TaskUpdates.m_IndexLookUp.TryGetValue(executor, out var index))
            {
                m_TaskUpdates.m_TickIntervalMultipliers[index] = tickIntervalMultiplier;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeTaskTickPausedInternal(ExecutorComponent executor, bool newValue)
        {
            if (m_TaskUpdates.m_IndexLookUp.TryGetValue(executor, out var index))
            {
                m_TaskUpdates.m_TickPaused[index] = newValue;
            }
        }
    }
}