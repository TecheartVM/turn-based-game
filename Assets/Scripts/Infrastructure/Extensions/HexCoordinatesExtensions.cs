using System.Collections.Generic;
using Base;
using Infrastructure.Utils;
using UnityEngine;

namespace Infrastructure.Extensions
{
    public static class HexCoordinatesExtensions
    {
        public static Vector3 ToVector3(this HexCoordinates hexCoordinates)
        {
            return new Vector3(hexCoordinates.x, 0, hexCoordinates.z);
        }

        public static Vector3 ToCubeCoordinates(this HexCoordinates hexCoordinates)
        {
            return Mathg.HexToCube(hexCoordinates.x, hexCoordinates.z);
        }
        
        public static HexCoordinates[] AxialRing(this HexCoordinates center, int radius)
        {
            Vector3 cube = center.OffsetToAxial().ToCubeCoordinates();
            Vector3 current = cube + Mathg.CubeDirection(4) * radius;

            List<HexCoordinates> result = new();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    result.Add(Mathg.CubeToAxial(current));
                    current = Mathg.CubeNeighbour(current, i);
                }
            }

            return result.ToArray();
        }

        public static HexCoordinates[] AxialRing(this HexCoordinates center, int minRadius, int maxRadius)
        {
            List<HexCoordinates> result = new();
            for (int i = minRadius; i <= maxRadius; i++)
                result.AddRange(AxialRing(center, i));
            return result.ToArray();
        }
    }
}