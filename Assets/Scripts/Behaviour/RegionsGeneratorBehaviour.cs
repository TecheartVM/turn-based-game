using System.Collections.Generic;
using Base;
using Behaviour.Structures;
using Infrastructure.Utils;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Behaviour
{
    [RequireComponent(typeof(TileGridBehaviour))]
    public class RegionsGeneratorBehaviour : MonoBehaviour
    {
        [SerializeField]
        private int mapWidth;
        [SerializeField]
        private int mapHeight;

        [SerializeField]
        private int minRegionSize = 16;
        [SerializeField]
        private int maxRegionSize = 22;

        [SerializeField]
        private PoissonHexSamplingSettings regionSamplingSettings;
        
        private TileGridBehaviour _grid;

        private void Awake()
        {
            _grid = GetComponent<TileGridBehaviour>();
            _grid.CreateGrid(mapHeight, mapWidth);
        }

        private void Update()
        {
            if (Input.GetButtonUp("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    HexCoordinates hitTile = GetTileCoordinates(hit.point);
                    GenerateMap(hitTile);
                }
            }
        }

        private void GenerateMap(HexCoordinates startAt)
        {
            List<HexCoordinates> unfilledTiles = new();
                    
            Region[] regions = GenerateRegions(startAt);
            HashSet<HexCoordinates> filledTiles = new HashSet<HexCoordinates>();
            foreach(var region in regions)
                filledTiles.AddRange(region.Tiles);

            for (int z = 0; z < mapHeight; z++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    HexCoordinates tile = new(x, z);
                    if(filledTiles.Contains(tile))
                        continue;
                    unfilledTiles.Add(tile);
                }
            }
            
            regions = PostProcessMap(regions, unfilledTiles.ToArray());

            foreach (var region in regions)
                if (region != Region.EMPTY)
                    ColorRegion(region);
        }

        private Region[] PostProcessMap(Region[] regions, HexCoordinates[] unfilledTiles)
        {
            Queue<HexCoordinates> tilesToProcess = new(unfilledTiles);

            while (tilesToProcess.Count > 0)
            {
                HexCoordinates tile = tilesToProcess.Dequeue();
                
                int bestDistance = int.MaxValue;
                Region bestRegion = Region.EMPTY;
                foreach (var region in regions)
                {
                    int distance = tile.OffsetDistance(region.Center);

                    if (distance <= 2)
                    {
                        bestRegion = region;
                        break;
                    }

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestRegion = region;
                    }
                    else if (distance == bestDistance)
                    {
                        if(bestRegion == Region.EMPTY)
                            continue;
                        
                        int tileCountComparison = bestRegion.Tiles.Count - region.Tiles.Count;
                        if (tileCountComparison > 0)
                        {
                            bestRegion = region;
                        }
                    }
                }

                if (bestRegion != Region.EMPTY)
                    bestRegion.Tiles.Add(tile);
                else tilesToProcess.Enqueue(tile);
            }

            return regions;
        }

        private Region[] GenerateRegions(HexCoordinates startingPoint)
        {
            List<Region> result = new();
            
            HexCoordinates extent = new(regionSamplingSettings.fieldExtent, regionSamplingSettings.fieldExtent);
            HexCoordinates padding = new(regionSamplingSettings.fieldPadding, regionSamplingSettings.fieldPadding);

            for (int i = 0; i < regionSamplingSettings.iterations; i++)
            {
                Vector2[] poissonPoints = Mathg.PoissonDiscSampling.SampleField(
                    new Vector2(mapWidth + extent.x * 2, mapHeight + extent.z * 2),
                    new Vector2(startingPoint.x, startingPoint.z),
                    regionSamplingSettings.maxSamplesPerNode,
                    regionSamplingSettings.minRadius + 1,
                    regionSamplingSettings.maxRadius + 1);
                
                foreach (var point in poissonPoints)
                {
                    HexCoordinates regionCenter = GetTileCoordinates(new Vector3(point.x, 0, point.y)) - extent;
                    if (!regionCenter.IsInBounds(padding, new HexCoordinates(mapWidth, mapHeight) - padding))
                        continue;
                
                    Region region = GenerateRegion(regionCenter);
                    if (region != Region.EMPTY)
                    {
                        ColorRegion(region);
                        result.Add(region);
                    }
                    
                }
            }

            return result.ToArray();
        }

        private void ColorRegion(Region region)
        {
            Color color = Random.ColorHSV();
            foreach (var tile in region.Tiles)
                _grid.SetTileColor(tile.x, tile.z, color);
                        
            _grid.SetTileColor(region.Center.x, region.Center.z, Color.red);
        }

        private Region GenerateRegion(HexCoordinates origin)
        {
            if(!IsFreeTile(origin))
                return Region.EMPTY;
            
            int size = Random.Range(minRegionSize, maxRegionSize + 1);

            var queue = new Queue<HexCoordinates>();
            queue.Enqueue(origin);
            
            Region result = new Region(origin);
            
            while (queue.Count > 0)
            {
                HexCoordinates cur = queue.Dequeue();
                result.Tiles.Add(cur);
                
                if(result.Tiles.Count >= size) break;

                foreach (var neighbour in cur.GetNeighbours())
                {
                    if (!IsFreeTile(neighbour))
                    {
                        // Abort region generation
                        // if there is no 1 tile radius empty area
                        if (result.Tiles.Count <= 1)
                            return Region.EMPTY;
                        
                        continue;
                    }
                    
                    if(Random.Range(0, 3) > 0)
                        if(!result.Tiles.Contains(neighbour))
                            queue.Enqueue(neighbour);
                }
            }

            return result;
        }

        public bool IsFreeTile(HexCoordinates atCoords)
        {
            return IsValidCoordinates(atCoords) && !IsRegionExistsAt(atCoords);
        }
        
        public bool IsRegionExistsAt(HexCoordinates coordinates)
        {
            return !_grid.GetTileColor(coordinates.x, coordinates.z).Equals(Color.white);
        }

        public bool IsValidCoordinates(HexCoordinates coords)
        {
            return coords.x >= 0 && coords.x < mapWidth && coords.z >= 0 && coords.z < mapHeight;
        }

        public HexCoordinates GetTileCoordinates(Vector3 worldSpacePos)
        {
            worldSpacePos -= transform.position;
            
            float r3 = TileProperties.OuterRadius * 3;
            float col = (Mathg.Sqrt3 * worldSpacePos.x - worldSpacePos.z) / r3;
            float row = 2 * worldSpacePos.z / r3;

            return Mathg.AxialRound(col, row).AxialToOffset();
        }
    }
}