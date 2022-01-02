﻿using System.Runtime.CompilerServices;
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
                m_TaskUpdates.CreateUpdateJob(deltaTime).Schedule(),
                m_ServiceUpdates.CreateUpdateJob(deltaTime).Schedule()
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
            var tickIntervals = (float*) m_TaskUpdates.m_TickIntervals.GetUnsafeReadOnlyPtr();
            var elapsedTimes = (float*) m_TaskUpdates.m_ElapsedTimes.GetUnsafePtr();

            // iterate backwards so we can remove executors as we go, if not needed
            for (var i = m_TaskUpdates.m_ObjectsCount - 1; i >= 0; i--)
            {
                var deltaTime = elapsedTimes[i];

                if (deltaTime < tickIntervals[i])
                {
                    continue;
                }

                // reset timer since it ran out
                elapsedTimes[i] = 0f;

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
            var tickIntervals = (float*) m_ServiceUpdates.m_TickIntervals.GetUnsafeReadOnlyPtr();
            var elapsedTimes = (float*) m_ServiceUpdates.m_ElapsedTimes.GetUnsafePtr();

            for (var i = m_ServiceUpdates.m_ObjectsCount - 1; i >= 0; i--)
            {
                var deltaTime = elapsedTimes[i];

                if (deltaTime < tickIntervals[i])
                {
                    continue;
                }

                // reset timer since it ran out
                elapsedTimes[i] = 0f;

                m_ServiceUpdates.m_ObjectsBuffer[i].WrappedTick(deltaTime);
            }
        }

        void ServiceRunner.IInterface.Add(IHiraBotsService service, float tickInterval)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferAddServiceCommand(service, tickInterval);
            }
            else
            {
                AddServiceInternal(service, tickInterval);
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

        void TaskRunner.IInterface.Add(ExecutorComponent executor, IHiraBotsTask task, float tickInterval)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferAddTaskCommand(executor, task, tickInterval);
            }
            else
            {
                AddTaskInternal(executor, task, tickInterval);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddServiceInternal(IHiraBotsService service, float tickInterval)
        {
            if (m_ServiceUpdates.Add(service, tickInterval))
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
        private void AddTaskInternal(ExecutorComponent executor, IHiraBotsTask task, float tickInterval)
        {
            if (m_TaskUpdates.Add(executor, tickInterval))
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
    }
}