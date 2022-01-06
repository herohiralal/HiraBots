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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float x, float y, float z) OffsetWToPositionW(int column, int height, int row, float innerRadius)
        {
            var heightParity = (math.abs(height * 3) + height) % 3;
            var rowParity = row & 1;

            var y = height * 1.632993162f; // ((2 * sqrt(2/3))) - keep shifting upwards for each unit height

            var z = ((1.732050807f * row) // 1.5 * 2 / sqrt(3) - keep shifting forwards for each unit row
                     + (heightParity * 0.577350269f)); // 1 / sqrt(3) - for odd heights, shift z to +ve

            var x = ((column * 2f) // 2 - keep shifting rightwards for each unit column
                     + (heightParity * 1f) // 1 - for odd heights, shift x to +ve
                     + (rowParity * 1f)); // 1 - for odd rows, shift x to +ve

            return ((x * innerRadius), (y * innerRadius), z * innerRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int column, int height, int row) PositionWToOffsetW(float x, float y, float z, float innerRadius)
        {
            var height = (int) math.round((y / innerRadius) / 1.632993162f);
            var heightParity = (math.abs(height * 3) + height) % 3;

            var row = (int) math.round(((z / innerRadius) - (heightParity * 0.577350269f)) / 1.732050807f);
            var rowParity = row & 1;

            var column = (int) math.round(((x / innerRadius) - (heightParity * 1f) - (rowParity * 1f)) / 2);

            return (column, height, row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int q, int s, int r) OffsetWToAxialW(int column, int height, int row)
        {
            var s = height;
            var heightParity = (math.abs(height * 3) + height) % 3;

            var r = row - ((height - heightParity) / 3);
            var rowParity = row & 1;

            var q = column - ((height - heightParity) / 3) - ((row - rowParity) / 2);

            return (q, s, r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int column, int height, int row) AxialWToOffsetW(int q, int s, int r)
        {
            var height = s;
            var heightParity = (math.abs(height * 3) + height) % 3;

            var row = r + ((height - heightParity) / 3);
            var rowParity = row & 1;

            var column = q + ((height - heightParity) / 3) + ((row - rowParity) / 2);

            return (column, height, row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanLength(int q, int s, int r)
        {
            return (math.abs(q) + math.abs(s) + math.abs(r) + math.abs(q + s + r)) / 2;
        }
    }
}