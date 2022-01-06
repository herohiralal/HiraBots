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