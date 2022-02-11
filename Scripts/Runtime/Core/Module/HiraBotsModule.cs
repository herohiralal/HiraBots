using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    [DefaultExecutionOrder(1000)]
    [AddComponentMenu("")]
    internal partial class HiraBotsModule : MonoBehaviour, TaskRunner.IInterface, ServiceRunner.IInterface, BehaviourUpdater.IInterface
    {
        private UpdateSystem<IUpdatableBehaviour> m_BehaviourUpdates;
        private UpdateSystem<IHiraBotsService> m_ServiceUpdates;
        private UpdateSystem<ExecutorComponent> m_TaskUpdates;

        private JobHandle? m_UpdateJob;

        private static HiraBotsModule s_Instance;

        private enum UpdateSystem : byte
        {
            None,
            Behaviours,
            Services,
            Tasks
        }

        private UpdateSystem m_UpdateSystemLockedForModifications = UpdateSystem.None;

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
            BehaviourUpdater.instance = this;
            TaskRunner.instance = this;
            ServiceRunner.instance = this;

            InitializeCommandBuffer();

            m_BehaviourUpdates = new UpdateSystem<IUpdatableBehaviour>(2);
            m_ServiceUpdates = new UpdateSystem<IHiraBotsService>(1);
            m_TaskUpdates = new UpdateSystem<ExecutorComponent>(1);

            m_UpdateJob = null;

            m_UpdateSystemLockedForModifications = UpdateSystem.None;

            try
            {
                PerceptionSystem.Initialize();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal void Dispose()
        {
            // singleton stuff
            if (s_Instance != this)
            {
                return;
            }

            try
            {
                PerceptionSystem.Shutdown();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            m_UpdateJob?.Complete();
            m_UpdateJob = null;

            m_UpdateSystemLockedForModifications = UpdateSystem.None;

            ApplyCommandBuffer();

            m_UpdateSystemLockedForModifications = UpdateSystem.Tasks;
            for (var i = 0; i < m_TaskUpdates.m_ObjectsCount; i++)
            {
                m_TaskUpdates.m_ObjectsBuffer[i].AbortTask();
            }

            m_TaskUpdates.Dispose();
            m_UpdateSystemLockedForModifications = UpdateSystem.None;

            m_UpdateSystemLockedForModifications = UpdateSystem.Services;
            for (var i = 0; i < m_ServiceUpdates.m_ObjectsCount; i++)
            {
                m_ServiceUpdates.m_ObjectsBuffer[i].WrappedStop();
            }

            m_ServiceUpdates.Dispose();
            m_UpdateSystemLockedForModifications = UpdateSystem.None;

            // no need to abort/stop anything inside behaviours
            m_BehaviourUpdates.Dispose();

            ShutdownCommandBuffer();

            ServiceRunner.instance = null;
            TaskRunner.instance = null;
            BehaviourUpdater.instance = null;
            CoroutineRunner.instance = null;
            s_Instance = null;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void FixedUpdate()
        {
            PerceptionSystem.UpdateDatabase();

            var tickPerceptionSystem = PerceptionSystem.shouldTick;

            if (tickPerceptionSystem)
            {
                PerceptionSystem.ScheduleJobs(Time.fixedDeltaTime);

                // do something here on the main thread if needed

                PerceptionSystem.CompleteJobs();
                PerceptionSystem.CollectJobResults();
            }
        }

        private void Update()
        {
            TickBehavioursUpdateSystem();
            TickTasksUpdateSystem();
            TickServicesUpdateSystem();

            var deltaTime = Time.deltaTime;

            m_UpdateJob = JobHandle.CombineDependencies
            (
                m_BehaviourUpdates.ScheduleTickJob(deltaTime),
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

        private unsafe void TickBehavioursUpdateSystem()
        {
            m_UpdateSystemLockedForModifications = UpdateSystem.Behaviours;

            var shouldTicks = (float*) m_BehaviourUpdates.m_ShouldTick.GetUnsafeReadOnlyPtr();

            // tick them in the order they were registered
            for (var i = 0; i < m_BehaviourUpdates.m_ObjectsCount; i++)
            {
                var deltaTime = shouldTicks[i];

                if (deltaTime <= 0f)
                {
                    continue;
                }

                m_BehaviourUpdates.m_ObjectsBuffer[i].WrappedTick(deltaTime);
            }

            m_UpdateSystemLockedForModifications = UpdateSystem.None;
            ApplyCommandBuffer();
        }

        private unsafe void TickTasksUpdateSystem()
        {
            m_UpdateSystemLockedForModifications = UpdateSystem.Tasks;

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

            m_UpdateSystemLockedForModifications = UpdateSystem.None;
            ApplyCommandBuffer();
        }

        private unsafe void TickServicesUpdateSystem()
        {
            m_UpdateSystemLockedForModifications = UpdateSystem.Services;

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

            m_UpdateSystemLockedForModifications = UpdateSystem.None;
            ApplyCommandBuffer();
        }

        void BehaviourUpdater.IInterface.Add(IUpdatableBehaviour behaviour, float tickInterval, float tickIntervalMultiplier)
        {
            if (m_UpdateJob.HasValue || m_UpdateSystemLockedForModifications == UpdateSystem.Behaviours)
            {
                BufferAddBehaviourCommand(behaviour, tickInterval, tickIntervalMultiplier);
            }
            else
            {
                AddBehaviourInternal(behaviour, tickInterval, tickIntervalMultiplier);
            }
        }

        void BehaviourUpdater.IInterface.Remove(IUpdatableBehaviour behaviour)
        {
            if (m_UpdateJob.HasValue || m_UpdateSystemLockedForModifications == UpdateSystem.Behaviours)
            {
                BufferRemoveBehaviourCommand(behaviour);
            }
            else
            {
                RemoveBehaviourInternal(behaviour);
            }
        }

        void BehaviourUpdater.IInterface.ChangeTimeElapsedSinceLastTick(IUpdatableBehaviour behaviour, float timeElapsed)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferChangeBehaviourTimeElapsedCommand(behaviour, timeElapsed);
            }
            else
            {
                ChangeBehaviourTimeElapsedInternal(behaviour, timeElapsed);
            }
        }

        void BehaviourUpdater.IInterface.ChangeTickInterval(IUpdatableBehaviour behaviour, float tickInterval)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferChangeBehaviourTickIntervalCommand(behaviour, tickInterval);
            }
            else
            {
                ChangeBehaviourTickIntervalInternal(behaviour, tickInterval);
            }
        }

        void BehaviourUpdater.IInterface.ChangeTickPaused(IUpdatableBehaviour behaviour, bool value)
        {
            if (m_UpdateJob.HasValue)
            {
                BufferChangeBehaviourTickPausedCommand(behaviour, value);
            }
            else
            {
                ChangeBehaviourTickPausedInternal(behaviour, value);
            }
        }

        void ServiceRunner.IInterface.Add(IHiraBotsService service, float tickInterval, float tickIntervalMultiplier)
        {
            if (m_UpdateJob.HasValue || m_UpdateSystemLockedForModifications == UpdateSystem.Services)
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
            if (m_UpdateJob.HasValue || m_UpdateSystemLockedForModifications == UpdateSystem.Services)
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
            if (m_UpdateJob.HasValue || m_UpdateSystemLockedForModifications == UpdateSystem.Tasks)
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
            if (m_UpdateJob.HasValue || m_UpdateSystemLockedForModifications == UpdateSystem.Tasks)
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
        private void AddBehaviourInternal(IUpdatableBehaviour behaviour, float tickInterval, float tickIntervalMultiplier)
        {
            if (m_BehaviourUpdates.Add(behaviour, tickInterval, tickIntervalMultiplier))
            {
                // set the elapsed time to a random number, to add some randomization to ticking
                m_BehaviourUpdates.m_ElapsedTimes[m_BehaviourUpdates.m_ObjectsCount - 1] = Random.Range(0f, tickInterval);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveBehaviourInternal(IUpdatableBehaviour behaviour)
        {
            m_BehaviourUpdates.Remove(behaviour);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeBehaviourTimeElapsedInternal(IUpdatableBehaviour behaviour, float timeElapsed)
        {
            if (m_BehaviourUpdates.m_IndexLookUp.TryGetValue(behaviour, out var index))
            {
                m_BehaviourUpdates.m_ElapsedTimes[index] = timeElapsed;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeBehaviourTickIntervalInternal(IUpdatableBehaviour behaviour, float tickInterval)
        {
            if (m_BehaviourUpdates.m_IndexLookUp.TryGetValue(behaviour, out var index))
            {
                m_BehaviourUpdates.m_TickIntervals[index] = tickInterval;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ChangeBehaviourTickPausedInternal(IUpdatableBehaviour behaviour, bool newValue)
        {
            if (m_BehaviourUpdates.m_IndexLookUp.TryGetValue(behaviour, out var index))
            {
                m_BehaviourUpdates.m_TickPaused[index] = newValue;
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