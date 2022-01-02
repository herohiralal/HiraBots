using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    [Unity.Burst.BurstCompile]
    [AddComponentMenu("")]
    internal class HiraBotsTaskRunner : MonoBehaviour
    {
        #region Singleton

        private static HiraBotsTaskRunner s_Instance;

        private void Awake()
        {
            // singleton stuff
            if (s_Instance != null)
            {
                Destroy(this);
                return;
            }

            s_Instance = this;

            // start with an array of 16, so there can be at least one float4x4
            m_ElapsedTimes = new NativeArray<float>(16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_Timers = new NativeArray<float>(16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            m_Executors = new List<ExecutorComponent>();
        }

        private void OnDestroy()
        {
            // singleton stuff
            if (s_Instance != this)
            {
                return;
            }

            s_Instance = null;

            // abort/clear all active tasks
            foreach (var executor in m_Executors) executor.AbortTask();
            m_Executors.Clear();
            m_Executors = null;

            // dispose native arrays
            m_Timers.Dispose();
            m_ElapsedTimes.Dispose();
        }

        #endregion

        private NativeArray<float> m_ElapsedTimes;
        private NativeArray<float> m_Timers;
        private List<ExecutorComponent> m_Executors;

        /// <summary>
        /// Add an executor to the task runner.
        /// </summary>
        internal static void Add(ExecutorComponent executor, float timer, IHiraBotsTask task)
        {
            var instance = s_Instance;

            // check if already running
            if (instance.m_Executors.Contains(executor))
            {
                return;
            }

            instance.m_Executors.Add(executor);

            var activeExecutorsCount = instance.m_Executors.Count;

            // reallocate if required
            var timersBufferSize = instance.m_ElapsedTimes.Length;
            if (timersBufferSize < activeExecutorsCount)
            {
                instance.m_ElapsedTimes.Reallocate(timersBufferSize * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                instance.m_Timers.Reallocate(timersBufferSize * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            }

            // set timer and reset elapsed time
            instance.m_ElapsedTimes[activeExecutorsCount - 1] = 0f;
            instance.m_Timers[activeExecutorsCount - 1] = timer;

            // start the actual task
            executor.BeginTask(task);
        }

        /// <summary>
        /// Remove an executor from the task runner.
        /// </summary>
        internal static void Remove(ExecutorComponent executor)
        {
            var instance = s_Instance;

            // check if even running
            var index = instance.m_Executors.IndexOf(executor);
            if (index == -1)
            {
                return;
            }

            // swap the index with the last one so we don't have to pay
            // a stupid memory moving cost of removing from the middle

            var lastIndex = instance.m_Executors.Count - 1;

            instance.m_Timers[index] = instance.m_Timers[lastIndex];
            instance.m_ElapsedTimes[index] = instance.m_Timers[lastIndex];

            instance.m_Executors[index] = instance.m_Executors[lastIndex];

            instance.m_Executors.RemoveAt(lastIndex);

            // abort the actual task
            executor.AbortTask();
        }

        private unsafe void Update()
        {
            // update all the elapsed timers
            new TimerUpdateJob(m_ElapsedTimes.Reinterpret<float4x4>(sizeof(float)), Time.deltaTime).Run();

            var timers = (float*) m_Timers.GetUnsafeReadOnlyPtr();
            var elapsedTimes = (float*) m_ElapsedTimes.GetUnsafePtr();

            for (var i = m_Executors.Count - 1; i >= 0; i--)
            {
                // ignore if still time remaining
                if (elapsedTimes[i] < timers[i])
                {
                    continue;
                }

                var deltaTime = elapsedTimes[i];

                // reset timer if it ran out
                elapsedTimes[i] = timers[i];

                var keepExecuting = m_Executors[i].Tick(deltaTime);

                if (!keepExecuting)
                {
                    // remove-at/swap-back
                    var lastIndex = m_Executors.Count - 1;
                    m_Executors[i] = m_Executors[lastIndex];
                    m_Executors.RemoveAt(lastIndex);
                }
            }
        }

        // helper job to update all the timers
        [Unity.Burst.BurstCompile]
        private struct TimerUpdateJob : IJob
        {
            internal TimerUpdateJob(NativeArray<float4x4> target, float deltaTime)
            {
                m_Target = target;
                m_DeltaTime = deltaTime;
            }

            private NativeArray<float4x4> m_Target;
            private readonly float m_DeltaTime;

            public void Execute()
            {
                for (var i = 0; i < m_Target.Length; i++)
                {
                    m_Target[i] += m_DeltaTime;
                }
            }
        }
    }
}