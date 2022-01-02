﻿using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace HiraBots
{
    internal partial class HiraBotsModule
    {
        internal struct UpdateSystem<T>
        {
            internal NativeArray<float> m_TickIntervals;
            internal NativeArray<float> m_ElapsedTimes;
            internal NativeArray<float> m_TimeDilationValues;
            private readonly Dictionary<T, int> m_IndexLookUp;
            internal T[] m_ObjectsBuffer;
            internal int m_ObjectsCount;

            internal UpdateSystem(int batchCount)
            {
                m_TickIntervals = new NativeArray<float>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_ElapsedTimes = new NativeArray<float>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_TimeDilationValues = new NativeArray<float>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_IndexLookUp = new Dictionary<T, int>(batchCount * 16);
                m_ObjectsBuffer = new T[batchCount * 16];
                m_ObjectsCount = 0;
            }

            internal void Dispose()
            {
                m_ObjectsCount = 0;
                m_ObjectsBuffer = new T[0];
                m_IndexLookUp.Clear();
                m_TimeDilationValues.Dispose();
                m_ElapsedTimes.Dispose();
                m_TickIntervals.Dispose();
            }

            internal bool Add(T obj, float tickInterval, float timeDilation)
            {
                if (m_IndexLookUp.ContainsKey(obj))
                {
                    return false;
                }

                if (m_ObjectsCount == m_ObjectsBuffer.Length)
                {
                    // reallocation time
                    m_TickIntervals.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    m_ElapsedTimes.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    m_TimeDilationValues.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    System.Array.Resize(ref m_ObjectsBuffer, m_ObjectsCount * 2);
                }

                m_TickIntervals[m_ObjectsCount] = tickInterval;
                m_ElapsedTimes[m_ObjectsCount] = 0f;
                m_TimeDilationValues[m_ObjectsCount] = timeDilation;
                m_IndexLookUp.Add(obj, m_ObjectsCount);
                m_ObjectsBuffer[m_ObjectsCount] = obj;
                m_ObjectsCount++;

                return true;
            }

            internal bool Remove(T obj)
            {
                if (!m_IndexLookUp.TryGetValue(obj, out var index))
                {
                    return false;
                }

                Remove(index);

                return true;
            }

            internal void Remove(int index)
            {
                var lastObjectIndex = m_ObjectsCount - 1;

                // remove / swap back
                m_TickIntervals[index] = m_TickIntervals[lastObjectIndex];
                m_ElapsedTimes[index] = m_ElapsedTimes[lastObjectIndex];
                m_TimeDilationValues[index] = m_TimeDilationValues[lastObjectIndex];
                m_IndexLookUp[m_ObjectsBuffer[lastObjectIndex]] = index;
                m_IndexLookUp.Remove(m_ObjectsBuffer[index]);
                m_ObjectsBuffer[index] = m_ObjectsBuffer[lastObjectIndex];
                m_ObjectsBuffer[lastObjectIndex] = default;
                m_ObjectsCount--;
            }

            internal void SetTimeDilation(T obj, float timeDilation)
            {
                if (!m_IndexLookUp.TryGetValue(obj, out var index))
                {
                    return;
                }

                m_TimeDilationValues[index] = timeDilation;
            }

            internal ElapsedTimeUpdateJob CreateUpdateJob(float deltaTime)
            {
                return new ElapsedTimeUpdateJob(
                    m_ElapsedTimes.Reinterpret<float4x4>(sizeof(float)),
                    m_TimeDilationValues.Reinterpret<float4x4>(sizeof(float)),
                    deltaTime);
            }
        }

        [Unity.Burst.BurstCompile]
        internal struct ElapsedTimeUpdateJob : IJob
        {
            internal ElapsedTimeUpdateJob(NativeArray<float4x4> elapsedTimes, NativeArray<float4x4> timeDilationValues, float deltaTime)
            {
                m_ElapsedTimes = elapsedTimes;
                m_TimeDilationValues = timeDilationValues;
                m_DeltaTime = deltaTime;
            }

            private NativeArray<float4x4> m_ElapsedTimes;
            [ReadOnly] private NativeArray<float4x4> m_TimeDilationValues;
            [ReadOnly] private readonly float m_DeltaTime;

            public void Execute()
            {
                Unity.Burst.CompilerServices.Hint.Assume((m_ElapsedTimes.Length % 16) == 0);
                Unity.Burst.CompilerServices.Hint.Assume(m_TimeDilationValues.Length == m_ElapsedTimes.Length);

                for (var i = 0; i < m_ElapsedTimes.Length; i++)
                {
                    m_ElapsedTimes[i] += m_DeltaTime * m_TimeDilationValues[i];
                }
            }
        }
    }
}