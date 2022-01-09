#if UNITY_EDITOR
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

            var c = Gizmos.color;
            Gizmos.color = Color.black;

            for (var height = 0; height < dimensions.y; height++)
            {
                for (var row = 0; row < dimensions.z; row++)
                {
                    for (var column = 0; column < dimensions.x; column += 4)
                    {
                        var offsetsL = new int3x4(
                            new int3(column + 0, height, row),
                            new int3(column + 1, height, row),
                            new int3(column + 2, height, row),
                            new int3(column + 3, height, row));

                        var offsetsW = new int3x4(pivot, pivot, pivot, pivot) + offsetsL;

                        var positionsW = TacMapUtility.OffsetWToPositionW(offsetsW, cellSize * 0.5f);

                        for (var i = 0; i < 4; i++)
                        {
                            Gizmos.DrawWireSphere(positionsW[i], cellSize * 0.05f);
                        }
                    }
                }
            }

            Gizmos.color = c;
        }

        internal void DrawGizmos(float valueA, float valueB, Color colorA, Color colorB)
        {
            if (m_ActiveWriteJob.HasValue)
            {
                return;
            }

            var (pivot, dimensions) = (m_Pivot, m_Dimensions);
            var cellSize = m_CellSize;

            var c = Gizmos.color;

            for (var height = 0; height < dimensions.y; height++)
            {
                for (var row = 0; row < dimensions.z; row++)
                {
                    for (var column = 0; column < dimensions.x; column += 4)
                    {
                        var offsetsL = new int3x4(
                            new int3(column + 0, height, row),
                            new int3(column + 1, height, row),
                            new int3(column + 2, height, row),
                            new int3(column + 3, height, row));

                        var offsetsW = new int3x4(pivot, pivot, pivot, pivot) + offsetsL;

                        var individualOffsetsL = math.transpose(offsetsL);

                        var arrayIndices = (individualOffsetsL.c1 * dimensions.x * dimensions.z) // adjust for height
                                           + (individualOffsetsL.c2 * dimensions.x) // adjust for row
                                           + individualOffsetsL.c0; // adjust for column

                        var positionsW = TacMapUtility.OffsetWToPositionW(offsetsW, m_CellSize * 0.5f);

                        for (var i = 0; i < 4; i++)
                        {
                            var value = m_Internal[arrayIndices[i]];
                            var t = Mathf.InverseLerp(valueA, valueB, value);

                            Gizmos.color = Color.LerpUnclamped(colorA, colorB, t);
                            Gizmos.DrawWireSphere(positionsW[i], cellSize * 0.05f);
                        }
                    }
                }
            }

            Gizmos.color = c;
        }
    }
}
#endif