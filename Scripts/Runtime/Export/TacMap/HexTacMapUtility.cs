using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine
{
    public static class HexTacMapUtility
    {
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
        public static float4x3 IndividualOffsetsWToLinearPositionsW(int4x3 individualOffsets, float innerRadius)
        {
            var columns = individualOffsets.c0;
            var heights = individualOffsets.c1;
            var rows = individualOffsets.c2;

            var heightParities = (math.abs(heights * 3) + heights) % 3;
            var rowParities = rows & 1;

            var y = heights * new float4(1.6329933162f);

            var z = ((rows * new float4(1.732050807f))
                     + (heightParities * new float4(0.577350269f)));

            var x = ((columns * new float4(2f))
                     + (heightParities * new float4(1f))
                     + (rowParities * new float4(1f)));

            return new float4x3(x * innerRadius, y * innerRadius, z * innerRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x3 OffsetWToLinearPositionsW(int3x4 offsetCoordinates, float innerRadius)
        {
            return IndividualOffsetsWToLinearPositionsW(math.transpose(offsetCoordinates), innerRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 IndividualOffsetsWToPositionW(int4x3 individualOffsets, float innerRadius)
        {
            return math.transpose(IndividualOffsetsWToLinearPositionsW(individualOffsets, innerRadius));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 OffsetWToPositionW(int3x4 offsetCoordinates, float innerRadius)
        {
            var linearPositions = IndividualOffsetsWToLinearPositionsW(math.transpose(offsetCoordinates), innerRadius);

            return math.transpose(linearPositions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int column, int height, int row) PositionWToOffsetW(float x, float y, float z, float innerRadius)
        {
            var heightF = (y / innerRadius) / 1.632993162f;
            var height = math.select((int) heightF, (int) (heightF + 1f), math.frac(heightF) >= 0.5f);
            var heightParity = (math.abs(height * 3) + height) % 3;

            var rowF = ((z / innerRadius) - (heightParity * 0.577350269f)) / 1.732050807f;
            var row = math.select((int) rowF, (int) (rowF + 1f), math.frac(rowF) >= 0.5f);
            var rowParity = row & 1;

            var columnF = ((x / innerRadius) - (heightParity * 1f) - (rowParity * 1f)) / 2;
            var column = math.select((int) columnF, (int) (columnF + 1f), math.frac(columnF) >= 0.5f);

            return (column, height, row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x3 LinearPositionsWToIndividualOffsetsW(float4x3 linearPositions, float innerRadius)
        {
            var x = linearPositions.c0;
            var y = linearPositions.c1;
            var z = linearPositions.c2;

            var heightF = (y / innerRadius) / 1.632993162f;
            var heights = math.select((int4) heightF, (int4) (heightF + 1f), math.frac(heightF) > 0.5f);
            var heightParities = (math.abs(heights * 3) + heights) % 3;

            var rowF = ((z / innerRadius) - (heightParities * new float4(0.577350269f))) / 1.732050807f;
            var rows = math.select((int4) rowF, (int4) (rowF + 1f), math.frac(rowF) > 0.5f);
            var rowParity = rows & 1;

            var columnF = ((x / innerRadius) - (heightParities * new float4(1f)) - (rowParity * new float4(1f))) / 2;
            var columnParities = math.select((int4) columnF, (int4) (columnF + 1f), math.frac(columnF) > 0.5f);

            return new int4x3(columnParities, heights, rows);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x3 PositionWToIndividualOffsetsW(float3x4 vectorCoordinates, float innerRadius)
        {
            return LinearPositionsWToIndividualOffsetsW(math.transpose(vectorCoordinates), innerRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x4 LinearPositionsWToOffsetW(float4x3 linearPositions, float innerRadius)
        {
            return math.transpose(LinearPositionsWToIndividualOffsetsW(linearPositions, innerRadius));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x4 PositionWToOffsetW(float3x4 vectorCoordinates, float innerRadius)
        {
            var individualOffsets = LinearPositionsWToIndividualOffsetsW(math.transpose(vectorCoordinates), innerRadius);

            return math.transpose(individualOffsets);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int q, int t, int r) OffsetWToAxialW(int column, int height, int row)
        {
            var t = height;
            var heightParity = (math.abs(height * 3) + height) % 3;

            var r = row - ((height - heightParity) / 3);
            var rowParity = row & 1;

            var q = column - ((height - heightParity) / 3) - ((row - rowParity) / 2);

            return (q, t, r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (int column, int height, int row) AxialWToOffsetW(int q, int t, int r)
        {
            var height = t;
            var heightParity = (math.abs(height * 3) + height) % 3;

            var row = r + ((height - heightParity) / 3);
            var rowParity = row & 1;

            var column = q + ((height - heightParity) / 3) + ((row - rowParity) / 2);

            return (column, height, row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanLength(int q, int t, int r)
        {
            return (math.abs(q) + math.abs(t) + math.abs(r) + math.abs(q + t + r)) / 2;
        }
    }
}