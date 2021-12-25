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
    internal class HiraBotsServiceRunner : MonoBehaviour
    {
        #region Singleton

        private static HiraBotsServiceRunner s_Instance;

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

            m_ActiveServices = new List<IHiraBotsService>();
        }

        private void OnDestroy()
        {
            // singleton stuff
            if (s_Instance != this)
            {
                return;
            }

            s_Instance = null;

            // stop/clear all active services
            foreach (var service in m_ActiveServices)
            {
                try
                {
                    service.Stop();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            m_ActiveServices.Clear();
            m_ActiveServices = null;

            // dispose native arrays
            m_Timers.Dispose();
            m_ElapsedTimes.Dispose();
        }

        #endregion

        private NativeArray<float> m_ElapsedTimes;
        private NativeArray<float> m_Timers;
        private List<IHiraBotsService> m_ActiveServices;

        /// <summary>
        /// Add a service to the service runner.
        /// </summary>
        internal static void Add(IHiraBotsService service, float timer)
        {
            var instance = s_Instance;

            // check if already running
            if (instance.m_ActiveServices.Contains(service))
            {
                return;
            }

            instance.m_ActiveServices.Add(service);

            var activeServicesCount = instance.m_ActiveServices.Count;

            // reallocate if required
            var timersBufferSize = instance.m_ElapsedTimes.Length;
            if (timersBufferSize < activeServicesCount)
            {
                instance.m_ElapsedTimes.Reallocate(timersBufferSize * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                instance.m_Timers.Reallocate(timersBufferSize * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            }

            // set timer and reset elapsed time
            instance.m_ElapsedTimes[activeServicesCount - 1] = 0f;
            instance.m_Timers[activeServicesCount - 1] = timer;

            // start the actual service
            try
            {
                service.Start();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Remove a service from the service runner.
        /// </summary>
        internal static void Remove(IHiraBotsService service)
        {
            var instance = s_Instance;

            // check if even running
            var index = instance.m_ActiveServices.IndexOf(service);
            if (index == -1)
            {
                return;
            }

            // swap the index with the last one so we don't have to pay
            // a stupid memory moving cost of removing from the middle

            var lastIndex = instance.m_ActiveServices.Count - 1;

            instance.m_Timers[index] = instance.m_Timers[lastIndex];
            instance.m_ElapsedTimes[index] = instance.m_Timers[lastIndex];

            instance.m_ActiveServices[index] = instance.m_ActiveServices[lastIndex];

            instance.m_ActiveServices.RemoveAt(lastIndex);

            // stop the actual service
            try
            {
                service.Stop();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        private unsafe void Update()
        {
            // update all the elapsed timers
            new TimerUpdateJob(m_ElapsedTimes.Reinterpret<float4x4>(sizeof(float4x4)), Time.deltaTime).Run();

            var activeServicesCount = m_ActiveServices.Count;

            var timers = (float*) m_Timers.GetUnsafeReadOnlyPtr();
            var elapsedTimes = (float*) m_ElapsedTimes.GetUnsafePtr();

            for (var i = 0; i < activeServicesCount; i++)
            {
                // ignore if still time remaining
                if (elapsedTimes[i] < timers[i])
                {
                    continue;
                }

                var deltaTime = elapsedTimes[i];

                // reset timer if it ran out
                elapsedTimes[i] = timers[i];

                // avoid a nasty exception
                try
                {
                    m_ActiveServices[i].Tick(deltaTime);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
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