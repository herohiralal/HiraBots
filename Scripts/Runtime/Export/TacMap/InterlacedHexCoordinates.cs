using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine
{
    [System.Serializable]
    public struct InterlacedHexCoordinates :
        System.IEquatable<InterlacedHexCoordinates>,
        System.IComparable<InterlacedHexCoordinates>
    {
        public static InterlacedHexCoordinates zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
            = new InterlacedHexCoordinates(0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InterlacedHexCoordinates(int column, int row)
        {
            m_Column = column;
            m_Row = row;
        }

        [SerializeField] private int m_Column;
        [SerializeField] private int m_Row;

        #region Accessors

        public int column
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Column;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Column = value;
        }

        public int row
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Row;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Row = value;
        }

        #endregion

        #region Equality

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(InterlacedHexCoordinates other)
        {
            return m_Column == other.m_Column && m_Row == other.m_Row;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is InterlacedHexCoordinates other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(InterlacedHexCoordinates a, InterlacedHexCoordinates b)
        {
            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(InterlacedHexCoordinates a, InterlacedHexCoordinates b)
        {
            return !(a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return (m_Column * 397) ^ m_Row;
            }
        }

        #endregion

        #region Comparison

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(InterlacedHexCoordinates other)
        {
            var columnComparison = m_Column.CompareTo(other.m_Column);
            if (columnComparison != 0) return columnComparison;

            var rowComparison = m_Row.CompareTo(other.m_Row);
            if (rowComparison != 0) return rowComparison;

            return 0;
        }

        #endregion

        #region Unity Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int2(InterlacedHexCoordinates i)
        {
            return new int2(i.column, i.row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator InterlacedHexCoordinates(int2 i)
        {
            return new InterlacedHexCoordinates(i.x, i.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Int(InterlacedHexCoordinates i)
        {
            return new Vector2Int(i.column, i.row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator InterlacedHexCoordinates(Vector2Int v)
        {
            return new InterlacedHexCoordinates(v.x, v.y);
        }

        #endregion

        #region Hex Coordinate Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator InterlacedHexCoordinates(AxialHexCoordinates a)
        {
            return new InterlacedHexCoordinates(2 * a.q + a.r, a.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator InterlacedHexCoordinates(OffsetHexCoordinates o)
        {
            return new InterlacedHexCoordinates((2 * o.column) + (o.parity), o.row);
        }

        #endregion

        #region Neighbours

        public InterlacedHexCoordinates leftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InterlacedHexCoordinates(column - 2, row);
        }

        public InterlacedHexCoordinates topLeftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InterlacedHexCoordinates(column - 1, row - 1);
        }

        public InterlacedHexCoordinates topRightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InterlacedHexCoordinates(column + 1, row - 1);
        }

        public InterlacedHexCoordinates rightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InterlacedHexCoordinates(column + 2, row);
        }

        public InterlacedHexCoordinates bottomRightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InterlacedHexCoordinates(column + 1, row + 1);
        }

        public InterlacedHexCoordinates bottomLeftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new InterlacedHexCoordinates(column - 1, row + 1);
        }

        #endregion

        #region Arithmetic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InterlacedHexCoordinates operator +(InterlacedHexCoordinates a, InterlacedHexCoordinates b)
        {
            return new InterlacedHexCoordinates(a.column + b.column, a.row + b.row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static InterlacedHexCoordinates operator -(InterlacedHexCoordinates a, InterlacedHexCoordinates b)
        {
            return new InterlacedHexCoordinates(a.column - b.column, a.row - b.row);
        }

        #endregion
    }
}