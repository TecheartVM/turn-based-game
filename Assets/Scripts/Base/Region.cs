using System;
using System.Collections.Generic;

namespace Base
{
    public struct Region : IEquatable<Region>
    {
        public static readonly Region EMPTY = new();
        
        public HashSet<HexCoordinates> Tiles { get; }
        
        public HexCoordinates Center { get; }

        public Region(HexCoordinates center)
        {
            Center = center;
            Tiles = new HashSet<HexCoordinates> { center };
        }

        public bool Equals(Region other)
        {
            return Equals(Tiles, other.Tiles) && Center.Equals(other.Center);
        }

        public override bool Equals(object obj)
        {
            return obj is Region other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tiles, Center);
        }

        public static bool operator ==(Region left, Region right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Region left, Region right)
        {
            return !left.Equals(right);
        }
    }
}