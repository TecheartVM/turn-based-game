using System;

namespace Base
{
    public readonly struct HexCoordinates : IEquatable<HexCoordinates>
    {
        public static readonly HexCoordinates Zero = new (0, 0);
        public static readonly HexCoordinates One = new (1, 1);
        
        public int x { get; }
        public int z { get; }

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        /**
         * Converts Axial coordinates to Offset coordinates.
         */
        public static HexCoordinates AxialToOffset(HexCoordinates axial)
        {
            var col = axial.x + (axial.z - (axial.z & 1)) / 2;
            var row = axial.z;
            return new HexCoordinates(col, row);
        }
        
        /**
         * Converts Offset coordinates to Axial coordinates.
         */
        public static HexCoordinates OffsetToAxial(HexCoordinates offset)
        {
            var q = offset.x - (offset.z - (offset.z & 1)) / 2;
            var r = offset.z;
            return new HexCoordinates(q, r);
        }
        
        /**
         * Converts Axial coordinates to Offset coordinates.
         */
        public HexCoordinates AxialToOffset()
        {
            return AxialToOffset(this);
        }
        
        /**
         * Converts Offset coordinates to Axial coordinates.
         */
        public HexCoordinates OffsetToAxial()
        {
            return OffsetToAxial(this);
        }
        
        /**
         * Gets all 6 neighbour coordinates of this coordinate.
         */
        public HexCoordinates[] GetNeighbours()
        {
            if (z % 2 == 0)
                return new HexCoordinates[]
                {
                    new (x + 1, z),
                    new (x, z + 1),
                    new (x - 1, z),
                    new (x, z - 1),
                    new (x - 1, z + 1),
                    new (x - 1, z - 1)
                };
            
            return new HexCoordinates[]
            {
                new (x + 1, z),
                new (x, z + 1),
                new (x - 1, z),
                new (x, z - 1),
                new (x + 1, z + 1),
                new (x + 1, z - 1)
            };
        }
        
        /**
         * Gets distance between cells in Axial-space coordinates.
         */
        public static int AxialDistance(HexCoordinates fromHex, HexCoordinates toHex)
        {
            HexCoordinates dif = fromHex - toHex;
            return (Math.Abs(dif.x) + Math.Abs(dif.x + dif.z) + Math.Abs(dif.z)) / 2;
        }
        
        /**
         * Gets distance between cells in Offset-space coordinates.
         */
        public static int OffsetDistance(HexCoordinates fromHex, HexCoordinates toHex)
        {
            return AxialDistance(fromHex.OffsetToAxial(), toHex.OffsetToAxial());
        }
        
        /**
         * Gets distance to specified cell in Axial-space coordinates.
         */
        public int AxialDistance(HexCoordinates toHex)
        {
            return AxialDistance(this, toHex);
        }
        
        /**
         * Gets distance to specified cell in Offset-space coordinates.
         */
        public int OffsetDistance(HexCoordinates toHex)
        {
            return OffsetDistance(this, toHex);
        }

        public bool IsInBounds(HexCoordinates minCoordinates, HexCoordinates maxCoordinates)
        {
            return x >= minCoordinates.x
                   && x <= maxCoordinates.x
                   && z >= minCoordinates.z
                   && z <= maxCoordinates.z;
        }
        
        public static HexCoordinates operator +(HexCoordinates a, HexCoordinates b)
        {
            return new HexCoordinates(a.x + b.x, a.z + b.z);
        }
        
        public static HexCoordinates operator -(HexCoordinates a, HexCoordinates b)
        {
            return new HexCoordinates(a.x - b.x, a.z - b.z);
        }
        
        public static HexCoordinates operator *(HexCoordinates a, HexCoordinates b)
        {
            return new HexCoordinates(a.x * b.x, a.z * b.z);
        }
        
        public static HexCoordinates operator *(HexCoordinates a, int b)
        {
            return new HexCoordinates(a.x * b, a.z * b);
        }
        
        public static HexCoordinates operator *(HexCoordinates a, float b)
        {
            return new HexCoordinates((int)(a.x * b), (int)(a.z * b));
        }
        
        public static HexCoordinates operator /(HexCoordinates a, int b)
        {
            return new HexCoordinates(a.x / b, a.z / b);
        }
        
        public static HexCoordinates operator /(HexCoordinates a, float b)
        {
            return new HexCoordinates((int)(a.x / b), (int)(a.z / b));
        }

        public static bool operator ==(HexCoordinates left, HexCoordinates right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HexCoordinates left, HexCoordinates right)
        {
            return !left.Equals(right);
        }

        public bool Equals(HexCoordinates other)
        {
            return x == other.x && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is HexCoordinates other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, z);
        }

        public override string ToString()
        {
            return $"{x},{z}";
        }
    }
}