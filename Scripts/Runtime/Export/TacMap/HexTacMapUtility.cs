using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine
{
    public static class HexTacMapUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OuterRadiusToInnerRadius(float f)
        {
            return f * 0.866025404f; // sqrt(3) / 2
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InnerRadiusToOuterRadius(float f)
        {
            return f * 1.154700538f; // 2 / sqrt(3)
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 OffsetToPositionL(OffsetHexCoordinates o, float innerRadius)
        {
            var x = (o.column + o.parity * 0.5f) * (innerRadius * 2f);
            var y = -o.row * (InnerRadiusToOuterRadius(innerRadius) * 1.5f);

            return new float2(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 OffsetToPositionW(OffsetHexCoordinates o, float innerRadius, float4x4 localToWorldMatrix)
        {
            var offsetToPositionL = OffsetToPositionL(o, innerRadius);

            return math.mul(localToWorldMatrix, new float4(offsetToPositionL.x, 0f, offsetToPositionL.y, 1f)).xyz;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OffsetHexCoordinates PositionLToOffset(float2 v, float innerRadius)
        {
            var row = Mathf.RoundToInt(-v.y / (InnerRadiusToOuterRadius(innerRadius) * 1.5f));
            var column = Mathf.RoundToInt((v.x / (innerRadius * 2f)) - ((row & 1) * 0.5f));

            return new OffsetHexCoordinates(column, row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OffsetHexCoordinates PositionWToOffset(float3 v, float innerRadius, float4x4 worldToLocalMatrix)
        {
            var positionL = math.mul(worldToLocalMatrix, new float4(v, 1f));

            return PositionLToOffset(positionL.xz, innerRadius);
        }
    }
}