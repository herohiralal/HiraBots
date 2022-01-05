using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine
{
    [System.Serializable]
    public struct OffsetHexCoordinates :
        System.IEquatable<OffsetHexCoordinates>,
        System.IComparable<OffsetHexCoordinates>
    {
        public static OffsetHexCoordinates zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
            = new OffsetHexCoordinates(0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OffsetHexCoordinates(int column, int row)
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

        public int parity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Row & 1;
        }

        #endregion

        #region Equality

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(OffsetHexCoordinates other)
        {
            return m_Column == other.m_Column && m_Row == other.m_Row;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is OffsetHexCoordinates other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(OffsetHexCoordinates a, OffsetHexCoordinates b)
        {
            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(OffsetHexCoordinates a, OffsetHexCoordinates b)
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
        public int CompareTo(OffsetHexCoordinates other)
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
        public static implicit operator int2(OffsetHexCoordinates o)
        {
            return new int2(o.column, o.row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OffsetHexCoordinates(int2 i)
        {
            return new OffsetHexCoordinates(i.x, i.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Int(OffsetHexCoordinates o)
        {
            return new Vector2Int(o.column, o.row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OffsetHexCoordinates(Vector2Int v)
        {
            return new OffsetHexCoordinates(v.x, v.y);
        }

        #endregion

        #region Hex Coordinate Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OffsetHexCoordinates(AxialHexCoordinates a)
        {
            return new OffsetHexCoordinates(a.q + ((a.r - (a.r & 1)) / 2), a.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OffsetHexCoordinates(InterlacedHexCoordinates i)
        {
            return new OffsetHexCoordinates((i.column - (i.row & 1)) / 2, i.row);
        }

        #endregion

        #region Neighbours

        public OffsetHexCoordinates leftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new OffsetHexCoordinates(column - 1, row);
        }

        public OffsetHexCoordinates topLeftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new OffsetHexCoordinates(column + parity - 1, row - 1);
        }

        public OffsetHexCoordinates topRightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new OffsetHexCoordinates(column + parity + 1, row - 1);
        }

        public OffsetHexCoordinates rightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new OffsetHexCoordinates(column + 1, row);
        }

        public OffsetHexCoordinates bottomRightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new OffsetHexCoordinates(column + parity + 1, row + 1);
        }

        public OffsetHexCoordinates bottomLeftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new OffsetHexCoordinates(column + parity - 1, row + 1);
        }

        #endregion

        #region Arithmetic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OffsetHexCoordinates operator +(OffsetHexCoordinates a, OffsetHexCoordinates b)
        {
            return new OffsetHexCoordinates(a.column + b.column, a.row + b.row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OffsetHexCoordinates operator -(OffsetHexCoordinates a, OffsetHexCoordinates b)
        {
            return new OffsetHexCoordinates(a.column - b.column, a.row - b.row);
        }

        #endregion
    }
}