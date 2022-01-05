using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine
{
    [System.Serializable]
    public struct AxialHexCoordinates :
        System.IEquatable<AxialHexCoordinates>,
        System.IComparable<AxialHexCoordinates>
    {
        public static AxialHexCoordinates zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
            = new AxialHexCoordinates(0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AxialHexCoordinates(int q, int r)
        {
            m_Q = q;
            m_R = r;
        }

        [SerializeField] private int m_Q;
        [SerializeField] private int m_R;

        #region Accessors

        public int q
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Q;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Q = value;
        }

        public int r
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_R;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_R = value;
        }

        public int s
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => -m_Q + -m_R;
        }

        #endregion

        #region Equality

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(AxialHexCoordinates other)
        {
            return m_Q == other.m_Q && m_R == other.m_R;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is AxialHexCoordinates other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(AxialHexCoordinates a, AxialHexCoordinates b)
        {
            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(AxialHexCoordinates a, AxialHexCoordinates b)
        {
            return !(a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return (m_Q * 397) ^ m_R;
            }
        }

        #endregion

        #region Comparison

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(AxialHexCoordinates other)
        {
            var qComparison = m_Q.CompareTo(other.m_Q);
            if (qComparison != 0) return qComparison;

            var rComparison = m_R.CompareTo(other.m_R);
            if (rComparison != 0) return rComparison;

            return 0;
        }

        #endregion

        #region Unity Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int2(AxialHexCoordinates a)
        {
            return new int2(a.q, a.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AxialHexCoordinates(int2 i)
        {
            return new AxialHexCoordinates(i.x, i.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int3(AxialHexCoordinates a)
        {
            return new int3(a.q, a.r, a.s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AxialHexCoordinates(int3 i)
        {
            return new AxialHexCoordinates(i.x, i.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2Int(AxialHexCoordinates a)
        {
            return new Vector2Int(a.q, a.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AxialHexCoordinates(Vector2Int v)
        {
            return new AxialHexCoordinates(v.x, v.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3Int(AxialHexCoordinates a)
        {
            return new Vector3Int(a.q, a.r, a.s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AxialHexCoordinates(Vector3Int v)
        {
            return new AxialHexCoordinates(v.x, v.y);
        }

        #endregion

        #region Hex Coordinate Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AxialHexCoordinates(InterlacedHexCoordinates i)
        {
            return new AxialHexCoordinates((i.column - i.row) / 2, i.row);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AxialHexCoordinates(OffsetHexCoordinates o)
        {
            return new AxialHexCoordinates(o.column - ((o.row - o.parity) / 2), o.row);
        }

        #endregion

        #region Neighbours

        public AxialHexCoordinates leftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new AxialHexCoordinates(q - 1, r);
        }

        public AxialHexCoordinates topLeftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new AxialHexCoordinates(q, r - 1);
        }

        public AxialHexCoordinates topRightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new AxialHexCoordinates(q + 1, r - 1);
        }

        public AxialHexCoordinates rightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new AxialHexCoordinates(q + 1, r);
        }

        public AxialHexCoordinates bottomRightNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new AxialHexCoordinates(q, r + 1);
        }

        public AxialHexCoordinates bottomLeftNeighbour
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new AxialHexCoordinates(q - 1, r + 1);
        }

        #endregion

        #region Arithmetic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AxialHexCoordinates operator +(AxialHexCoordinates a, AxialHexCoordinates b)
        {
            return new AxialHexCoordinates(a.q + b.q, a.r + b.r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AxialHexCoordinates operator -(AxialHexCoordinates a, AxialHexCoordinates b)
        {
            return new AxialHexCoordinates(a.q - b.q, a.r - b.r);
        }

        public int manhattanDistanceToCenter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (math.abs(q) + math.abs(r) + math.abs(s)) / 2;
        }

        #endregion
    }
}