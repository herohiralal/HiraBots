﻿using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace HiraBots
{
    internal static class TacMapUtility
    {
        private static readonly float4x4[] s_CornersLooney =
        {
            new float4x4(
                new float4(-0.5f, -0.5f, -0.5f, 1),
                new float4(-0.5f, -0.5f, 0.5f, 1),
                new float4(-0.5f, 0.5f, -0.5f, 1),
                new float4(-0.5f, 0.5f, 0.5f, 1)),
            new float4x4(
                new float4(0.5f, -0.5f, -0.5f, 1),
                new float4(0.5f, -0.5f, 0.5f, 1),
                new float4(0.5f, 0.5f, -0.5f, 1),
                new float4(0.5f, 0.5f, 0.5f, 1))
        };

        internal static (int3 pivot, int3 dimensions) TransformToOffsetWBounds(float4x4 l2W, float cellSize)
        {
            int3 defaultMin = new int3(int.MaxValue), defaultMax = new int3(int.MinValue);
            var output = new int3x2(defaultMin, defaultMax);

            for (var i = 0; i < s_CornersLooney.Length; i++)
            {
                // get world space corners
                var cornersWPosLooney = math.mul(l2W, s_CornersLooney[i]);
                var cornersWPos = new float3x4(cornersWPosLooney.c0.xyz, cornersWPosLooney.c1.xyz, cornersWPosLooney.c2.xyz, cornersWPosLooney.c3.xyz);

                var cornersW = PositionWToOffsetW(cornersWPos, cellSize * 0.5f);

                output[0] = math.min(output[0], math.min(math.min(cornersW[0], cornersW[1]), math.min(cornersW[2], cornersW[3])));
                output[1] = math.max(output[1], math.max(math.max(cornersW[0], cornersW[1]), math.max(cornersW[2], cornersW[3])));
            }

            var pivot = output[0];

            var dimensions = output[1] + 1 - output[0];

            // make dimensions divisible by 4
            switch (dimensions.x % 4)
            {
                case 1:
                    pivot.x -= 2;
                    dimensions.x += 3;
                    break;
                case 2:
                    pivot.x -= 1;
                    dimensions.x += 2;
                    break;
                case 3:
                    pivot.x -= 1;
                    dimensions.x += 1;
                    break;
            }

            return (pivot, dimensions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float3 OffsetWToPositionW(int3 offsetCoordinates, float innerRadius)
        {
            var column = offsetCoordinates.x;
            var height = offsetCoordinates.y;
            var row = offsetCoordinates.z;

            var heightParity = (math.abs(height * 3) + height) % 3;
            var rowParity = row & 1;

            var y = height * 1.632993162f; // ((2 * sqrt(2/3))) - keep shifting upwards for each unit height

            var z = ((1.732050807f * row) // 1.5 * 2 / sqrt(3) - keep shifting forwards for each unit row
                     + (heightParity * 0.577350269f)); // 1 / sqrt(3) - for odd heights, shift z to +ve

            var x = ((column * 2f) // 2 - keep shifting rightwards for each unit column
                     + (heightParity * 1f) // 1 - for odd heights, shift x to +ve
                     + (rowParity * 1f)); // 1 - for odd rows, shift x to +ve

            return new float3(x, y, z) * innerRadius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float4x3 IndividualOffsetsWToLinearPositionsW(int4x3 individualOffsets, float innerRadius)
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
        internal static float4x3 OffsetWToLinearPositionsW(int3x4 offsetCoordinates, float innerRadius)
        {
            return IndividualOffsetsWToLinearPositionsW(math.transpose(offsetCoordinates), innerRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float3x4 IndividualOffsetsWToPositionW(int4x3 individualOffsets, float innerRadius)
        {
            return math.transpose(IndividualOffsetsWToLinearPositionsW(individualOffsets, innerRadius));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float3x4 OffsetWToPositionW(int3x4 offsetCoordinates, float innerRadius)
        {
            var linearPositions = IndividualOffsetsWToLinearPositionsW(math.transpose(offsetCoordinates), innerRadius);

            return math.transpose(linearPositions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int3 PositionWToOffsetW(float3 vectorCoordinates, float innerRadius)
        {
            var x = vectorCoordinates.x;
            var y = vectorCoordinates.y;
            var z = vectorCoordinates.z;

            var heightF = (y / innerRadius) / 1.632993162f;
            var height = math.select((int) heightF, (int) (heightF + 1f), math.frac(heightF) >= 0.5f);
            var heightParity = (math.abs(height * 3) + height) % 3;

            var rowF = ((z / innerRadius) - (heightParity * 0.577350269f)) / 1.732050807f;
            var row = math.select((int) rowF, (int) (rowF + 1f), math.frac(rowF) >= 0.5f);
            var rowParity = row & 1;

            var columnF = ((x / innerRadius) - (heightParity * 1f) - (rowParity * 1f)) / 2;
            var column = math.select((int) columnF, (int) (columnF + 1f), math.frac(columnF) >= 0.5f);

            return new int3(column, height, row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int4x3 LinearPositionsWToIndividualOffsetsW(float4x3 linearPositions, float innerRadius)
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
        internal static int4x3 PositionWToIndividualOffsetsW(float3x4 vectorCoordinates, float innerRadius)
        {
            return LinearPositionsWToIndividualOffsetsW(math.transpose(vectorCoordinates), innerRadius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int3x4 LinearPositionsWToOffsetW(float4x3 linearPositions, float innerRadius)
        {
            return math.transpose(LinearPositionsWToIndividualOffsetsW(linearPositions, innerRadius));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int3x4 PositionWToOffsetW(float3x4 vectorCoordinates, float innerRadius)
        {
            var individualOffsets = LinearPositionsWToIndividualOffsetsW(math.transpose(vectorCoordinates), innerRadius);

            return math.transpose(individualOffsets);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int3 OffsetWToAxialW(int3 offsetCoordinates)
        {
            var column = offsetCoordinates.x;
            var height = offsetCoordinates.y;
            var row = offsetCoordinates.z;

            var t = height;
            var heightParity = (math.abs(height * 3) + height) % 3;

            var r = row - ((height - heightParity) / 3);
            var rowParity = row & 1;

            var q = column - ((height - heightParity) / 3) - ((row - rowParity) / 2);

            return new int3(q, t, r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int3 AxialWToOffsetW(int3 axialCoordinates)
        {
            var q = axialCoordinates.x;
            var t = axialCoordinates.y;
            var r = axialCoordinates.z;

            var height = t;
            var heightParity = (math.abs(height * 3) + height) % 3;

            var row = r + ((height - heightParity) / 3);
            var rowParity = row & 1;

            var column = q + ((height - heightParity) / 3) + ((row - rowParity) / 2);

            return new int3(column, height, row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ManhattanLength(int3 axialCoordinates)
        {
            return (math.abs(axialCoordinates.x) + math.abs(axialCoordinates.y) + math.abs(axialCoordinates.z)
                    + math.abs(axialCoordinates.x + axialCoordinates.y + axialCoordinates.z)) / 2;
        }
    }
}