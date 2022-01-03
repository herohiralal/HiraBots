using System.Collections.Generic;
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
            internal NativeArray<float> m_TickIntervalMultipliers;
            internal NativeArray<bool> m_TickPaused;
            internal NativeArray<float> m_ElapsedTimes;
            internal NativeArray<float> m_ShouldTick;
            internal readonly Dictionary<T, int> m_IndexLookUp;
            internal T[] m_ObjectsBuffer;
            internal int m_ObjectsCount;

            internal UpdateSystem(int batchCount)
            {
                m_TickIntervals = new NativeArray<float>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_TickIntervalMultipliers = new NativeArray<float>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_TickPaused = new NativeArray<bool>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_ElapsedTimes = new NativeArray<float>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                m_ShouldTick = new NativeArray<float>(batchCount * 16, Allocator.Persistent, NativeArrayOptions.ClearMemory);
                m_IndexLookUp = new Dictionary<T, int>(batchCount * 16);
                m_ObjectsBuffer = new T[batchCount * 16];
                m_ObjectsCount = 0;
            }

            internal void Dispose()
            {
                m_ObjectsCount = 0;
                m_ObjectsBuffer = new T[0];
                m_IndexLookUp.Clear();
                m_ShouldTick.Dispose();
                m_ElapsedTimes.Dispose();
                m_TickPaused.Dispose();
                m_TickIntervalMultipliers.Dispose();
                m_TickIntervals.Dispose();
            }

            internal bool Add(T obj, float tickInterval, float tickIntervalMultiplier)
            {
                if (m_IndexLookUp.ContainsKey(obj))
                {
                    return false;
                }

                if (m_ObjectsCount == m_ObjectsBuffer.Length)
                {
                    // reallocation time
                    m_TickIntervals.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    m_TickIntervalMultipliers.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    m_TickPaused.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    m_ElapsedTimes.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                    m_ShouldTick.Reallocate(m_ObjectsCount * 2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
                    System.Array.Resize(ref m_ObjectsBuffer, m_ObjectsCount * 2);
                }

                m_TickIntervals[m_ObjectsCount] = tickInterval;
                m_TickIntervalMultipliers[m_ObjectsCount] = tickIntervalMultiplier;
                m_TickPaused[m_ObjectsCount] = false;
                m_ElapsedTimes[m_ObjectsCount] = 0f;
                m_ShouldTick[m_ObjectsCount] = -1f;
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
                m_TickIntervalMultipliers[index] = m_TickIntervalMultipliers[lastObjectIndex];
                m_TickPaused[index] = m_TickPaused[lastObjectIndex];
                m_ElapsedTimes[index] = m_ElapsedTimes[lastObjectIndex];
                m_ShouldTick[index] = m_ShouldTick[lastObjectIndex];
                m_IndexLookUp[m_ObjectsBuffer[lastObjectIndex]] = index;
                m_IndexLookUp.Remove(m_ObjectsBuffer[index]);
                m_ObjectsBuffer[index] = m_ObjectsBuffer[lastObjectIndex];
                m_ObjectsBuffer[lastObjectIndex] = default;
                m_ObjectsCount--;
            }

            internal JobHandle ScheduleTickJob(float deltaTime)
            {
                return new TickJob(
                        m_TickIntervals.Reinterpret<float4x4>(sizeof(float)),
                        m_TickIntervalMultipliers.Reinterpret<float4x4>(sizeof(float)),
                        m_TickPaused.Reinterpret<bool4x4>(sizeof(bool)),
                        m_ElapsedTimes.Reinterpret<float4x4>(sizeof(float)),
                        m_ShouldTick.Reinterpret<float4x4>(sizeof(float)),
                        deltaTime)
                    .Schedule();
            }
        }

        [Unity.Burst.BurstCompile]
        private struct TickJob : IJob
        {
            internal TickJob(
                NativeArray<float4x4> tickIntervals,
                NativeArray<float4x4> tickIntervalMultipliers,
                NativeArray<bool4x4> tickPaused,
                NativeArray<float4x4> elapsedTimes,
                NativeArray<float4x4> shouldTick,
                float deltaTime)
            {
                m_TickIntervals = tickIntervals;
                m_TickIntervalMultipliers = tickIntervalMultipliers;
                m_TickPaused = tickPaused;
                m_ElapsedTimes = elapsedTimes;
                m_ShouldTick = shouldTick;
                m_DeltaTime = deltaTime;
            }

            [ReadOnly] private readonly NativeArray<float4x4> m_TickIntervals;
            [ReadOnly] private readonly NativeArray<float4x4> m_TickIntervalMultipliers;
            [ReadOnly] private readonly NativeArray<bool4x4> m_TickPaused;
            private NativeArray<float4x4> m_ElapsedTimes;
            private NativeArray<float4x4> m_ShouldTick;
            [ReadOnly] private readonly float m_DeltaTime;

            public void Execute()
            {
                var length = m_TickIntervals.Length;

                for (var i = 0; i < length; i++)
                {
                    var elapsedTime = m_ElapsedTimes[i];
                    var effectiveTickInterval = (m_TickIntervals[i] * m_TickIntervalMultipliers[i]);
                    var shouldTick = elapsedTime > effectiveTickInterval;

                    m_ShouldTick[i] = Select(-1f, elapsedTime, shouldTick);
                    m_ElapsedTimes[i] = Select(m_DeltaTime, 0f, m_TickPaused[i]) // delta time
                                        + Select(elapsedTime, 0f, shouldTick); // original value
                }
            }

            private static float4x4 Select(float4x4 a, float4x4 b, bool4x4 c)
            {
                return new float4x4(
                    math.select(a.c0, b.c0, c.c0),
                    math.select(a.c1, b.c1, c.c1),
                    math.select(a.c2, b.c2, c.c2),
                    math.select(a.c3, b.c3, c.c3));
            }
        }
    }
}