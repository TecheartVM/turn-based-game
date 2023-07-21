using System;
using System.Collections.Generic;
using Base;
using Infrastructure.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Infrastructure.Utils
{
    public static class Mathg
    {
        public const float Sqrt2 = 1.41421356237f;
        public const float Sqrt3 = 1.73205080757f;
        public const float Sqrt2Inv = 0.70710678118f;
        public const float Sqrt3Inv = 0.57735026919f;
        
        /**
         * Converts hex Offset- or Axial-space 2D coordinates to Cube-space 3D hex coordinates.
         */
        public static Vector3 HexToCube(float x, float z) => new (x, -x - z, z);
        
        /**
         * Converts hex Cube-space 3D coordinates to hex Axial-space 2D coordinates.
         */
        public static HexCoordinates CubeToAxial(Vector3 cubeCoordinates) => 
            new HexCoordinates((int)cubeCoordinates.x, (int)cubeCoordinates.z);
        
        private static readonly Vector3[] CubeDirections =
        {
            new(1,0,-1), new(1, -1, 0), new(0, -1, 1), 
            new(-1, 0, 1), new(-1, 1, 0), new(0, 1, -1)
        };
        public static Vector3 CubeDirection(int index) => CubeDirections[index];

        public static Vector3 CubeNeighbour(Vector3 cubeCell, int directionIndex) =>
            cubeCell + CubeDirection(directionIndex);
        
        /**
         * Rounds hex Axial-space 2D coordinates. 
         */
        public static HexCoordinates AxialRound(float x, float z)
        {
            Vector3 cubeCoords = HexToCube(x, z);
            
            var col = Mathf.Round(cubeCoords.x);
            var row = Mathf.Round(cubeCoords.z);
            var off = Mathf.Round(cubeCoords.y);

            var qDiff = Mathf.Abs(col - cubeCoords.x);
            var rDiff = Mathf.Abs(row - cubeCoords.z);
            var sDiff = Mathf.Abs(off - cubeCoords.y);

            if (qDiff > rDiff && qDiff > sDiff)
                col = -row - off;
            else if (rDiff > sDiff)
                row = -col - off;

            return new HexCoordinates((int)col, (int)row);
        }

        public static class PoissonDiscSampling
        {
            public static Vector2[] SampleField(Vector2 fieldSize, Vector2 startAt, int maxSamples, float minRadius)
            {
                float maxRadius = minRadius * 2;
                return SampleField(fieldSize, startAt, maxSamples, minRadius, maxRadius);
            }
            
            public static Vector2[] SampleField(
                Vector2 fieldSize, Vector2 startAt, int maxSamples, float minRadius, float maxRadius)
            {
                float cellSize = minRadius / Sqrt2;

                int gridSizeX = Mathf.CeilToInt(fieldSize.x / cellSize);
                int gridSizeY = Mathf.CeilToInt(fieldSize.y / cellSize);
                int[,] grid = new int[gridSizeX, gridSizeY];

                List<Vector2> result = new ();
                List<Vector2> nodes = new () { startAt };

                while (nodes.Count > 0)
                {
                    int nodeIndex = Random.Range(0, nodes.Count);
                    Vector2 currentNode = nodes[nodeIndex];

                    bool sampleAccepted = false;

                    for (int i = 0; i < maxSamples; i++)
                    {
                        Vector2 sample = Sample(currentNode, minRadius, maxRadius);
                        if (IsSampleValid(sample, fieldSize, cellSize, minRadius, result, grid))
                        {
                            result.Add(sample);
                            nodes.Add(sample);
                            grid[(int)(sample.x / cellSize), (int)(sample.y / cellSize)] = result.Count;
                            sampleAccepted = true;
                            break;
                        }
                    }
                    
                    if(!sampleAccepted)
                        nodes.RemoveAt(nodeIndex);
                }

                return result.ToArray();
            }

            private static Vector2 Sample(Vector2 fromNode, float minRadius, float maxRadius)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                return fromNode + direction * Random.Range(minRadius, maxRadius);
            }

            private static bool IsSampleValid(Vector2 sample, Vector2 fieldSize, float cellSize, float radius,
                List<Vector2> acceptedPoints, int[,] grid)
            {
                if (sample.x < 0 || sample.x >= fieldSize.x || sample.y < 0 || sample.y >= fieldSize.y)
                    return false;

                int cellX = (int)(sample.x / cellSize);
                int cellY = (int)(sample.y / cellSize);

                int minX = Mathf.Max(0, cellX - 2);
                int maxX = Mathf.Min(grid.GetLength(0) - 1, cellX + 2);
                int minY = Mathf.Max(0, cellY - 2);
                int maxY = Mathf.Min(grid.GetLength(1) - 1, cellY + 2);

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        int pointIndex = grid[x, y] - 1;
                        if(pointIndex == -1)
                            continue;

                        float distance = Vector2.Distance(sample, acceptedPoints[pointIndex]);
                        if (distance < radius)
                            return false;
                    }
                }

                return true;
            }
        }
    }
}

//for (int z = Math.Max(sample.z - minDistance - 1, 0); z < Math.Min(sample.z + maxDistance + 1, fieldSize.z) && !sampleNearby; z++)
//{
//    for (int x = Math.Max(sample.x - minDistance - 1, 0); x < Math.Min(sample.x + maxDistance + 1, fieldSize.x); x++)
//    {
//        if (grid[x, z])
//        {
//            sampleNearby = true;
//            break;
//        }
//    }
//}

//HexCoordinates[] ring = sample.AxialRing(1, minDistance);
//foreach (var axial in ring)
//{
//    HexCoordinates offset = axial.AxialToOffset();
//    if (!offset.IsInBounds(HexCoordinates.Zero, fieldSize))
//        continue;
//    
//    if (grid[offset.x, offset.z])
//    {
//        sampleNearby = true;
//        break;
//    }
//}