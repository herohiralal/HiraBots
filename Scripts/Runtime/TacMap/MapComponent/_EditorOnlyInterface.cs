#if UNITY_EDITOR
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HiraBots
{
    internal partial class TacMapComponent
    {
        internal static void DrawGizmosDisabled(Transform t, float cellSize)
        {
            var (pivot, dimensions) = TacMapUtility.TransformToOffsetWBounds(t.localToWorldMatrix, cellSize);

            if (dimensions.x * dimensions.y <= 0 || dimensions.y * dimensions.z <= 0)
            {
                return;
            }

            if (UnsafeUtility.SizeOf<Vector3>() != UnsafeUtility.SizeOf<float3>())
            {
                Debug.LogError($"Size of UnityEngine.Vector3 is not the same as the size of Unity.Mathematics.float3.");
                return;
            }

            var c = Gizmos.color;
            Gizmos.color = Color.black;

            using (var arr = new NativeArray<Vector3>(dimensions.x * dimensions.y * dimensions.z, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
            {
                new PositionCalculatorJob(pivot, dimensions, cellSize, arr.Reinterpret<float3x4>(UnsafeUtility.SizeOf<Vector3>())).Run();

                foreach (var pos in arr)
                {
                    Gizmos.DrawWireSphere(pos, cellSize * 0.05f);
                }
            }

            Gizmos.color = c;
        }

        [BurstCompile]
        private struct PositionCalculatorJob : IJob
        {
            internal PositionCalculatorJob(int3 pivot, int3 dimensions, float cellSize, NativeArray<float3x4> output)
            {
                m_Pivot = pivot;
                m_Dimensions = dimensions;
                m_CellSize = cellSize;
                m_Output = output;
            }

            [ReadOnly] private readonly int3 m_Pivot;
            [ReadOnly] private readonly int3 m_Dimensions;
            [ReadOnly] private readonly float m_CellSize;
            [WriteOnly] private NativeArray<float3x4> m_Output;

            public void Execute()
            {
                for (var i = 0; i < m_Output.Length; i++)
                {
                    var height = (i * 4) / (m_Dimensions.x * m_Dimensions.z);
                    var row = ((i * 4) / m_Dimensions.x) % m_Dimensions.z;
                    var column = (i * 4) % m_Dimensions.x;

                    var individualOffsetsL = new int4x3(
                        new int4(column + 0, column + 1, column + 2, column + 3),
                        height,
                        row);

                    var individualOffsetsW = new int4x3(m_Pivot.x, m_Pivot.y, m_Pivot.z) + individualOffsetsL;

                    var positionsW = math.transpose(TacMapUtility.IndividualOffsetsWToLinearPositionsW(individualOffsetsW, m_CellSize * 0.5f));

                    m_Output[i] = positionsW;
                }
            }
        }

        internal void DrawGizmos(float valueA, float valueB, Color colorA, Color colorB)
        {
            if (m_ActiveWriteJob.HasValue)
            {
                return;
            }

            if (UnsafeUtility.SizeOf<Vector3>() != UnsafeUtility.SizeOf<float3>())
            {
                Debug.LogError($"Size of UnityEngine.Vector3 is not the same as the size of Unity.Mathematics.float3.");
                return;
            }

            if (UnsafeUtility.SizeOf<Color>() != UnsafeUtility.SizeOf<float4>())
            {
                Debug.LogError($"Size of UnityEngine.Color is not the same as the size of Unity.Mathematics.float4.");
                return;
            }

            var c = Gizmos.color;

            RequestDataForReadJob((m, p, d, s) =>
            {
                using (var pos = new NativeArray<Vector3>(m.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                {
                    new PositionCalculatorJob(p, d, s, pos.Reinterpret<float3x4>(UnsafeUtility.SizeOf<Vector3>())).Run();

                    using (var col = new NativeArray<Color>(m.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                    {
                        new ColorCalculatorJob(m.Reinterpret<float4>(sizeof(float)),
                                valueA, valueB, (Vector4) colorA, (Vector4) colorB,
                                col.Reinterpret<float4x4>(UnsafeUtility.SizeOf<Color>()))
                            .Run();

                        for (var i = 0; i < m.Length; i++)
                        {
                            Gizmos.color = col[i];
                            Gizmos.DrawWireSphere(pos[i], m_CellSize * 0.05f);
                        }
                    }
                }
            });

            Gizmos.color = c;
        }

        [BurstCompile]
        private struct ColorCalculatorJob : IJob
        {
            internal ColorCalculatorJob(NativeArray<float4> map,
                float valueA, float valueB, float4 colorA, float4 colorB,
                NativeArray<float4x4> outputColors)
            {
                m_Map = map;
                m_ValueA = valueA;
                m_ValueB = valueB;
                m_ColorA = colorA;
                m_ColorB = colorB;
                m_OutputColors = outputColors;
            }

            [ReadOnly] private readonly NativeArray<float4> m_Map;

            [ReadOnly] private readonly float m_ValueA;
            [ReadOnly] private readonly float m_ValueB;
            [ReadOnly] private readonly float4 m_ColorA;
            [ReadOnly] private readonly float4 m_ColorB;
            [WriteOnly] private NativeArray<float4x4> m_OutputColors;

            public void Execute()
            {
                for (var i = 0; i < m_OutputColors.Length; i++)
                {
                    var t = math.unlerp(m_ValueA, m_ValueB, m_Map[i]);

                    var colorR = math.lerp(m_ColorA.x, m_ColorB.x, t);
                    var colorG = math.lerp(m_ColorA.y, m_ColorB.y, t);
                    var colorB = math.lerp(m_ColorA.z, m_ColorB.z, t);

                    var colors = new float4x4(colorR, colorG, colorB, 1);

                    m_OutputColors[i] = math.transpose(colors);
                }
            }
        }
    }
}
#endif