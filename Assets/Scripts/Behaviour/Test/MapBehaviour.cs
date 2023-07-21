using Base;
using Base.Test;
using UnityEngine;

namespace Behaviour.Test
{
    public class MapBehaviour : MonoBehaviour
    {
        [SerializeField]
        private int mapWidth;
        [SerializeField]
        private int mapHeight;
        [SerializeField]
        [Range(1, 8)]
        private int noiseOctaves = 4;
        [SerializeField]
        private float noiseAmplitudeChange = 0.5f;
        [SerializeField]
        private float noiseFrequencyChange = 2.0f;

        [SerializeField]
        [Range(1.0f, 10.0f)]
        private float noiseScale = 7.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float mountainousness = 0.6f;

        [SerializeField]
        private TileBehaviour[] tilePrefabs;

        private TileBehaviour[,] _tiles;
        private float[,] _noise;
        
        private NoiseGenerator _noiseGenerator = new (Mathf.PerlinNoise);
        private Vector2 _noiseOffset;
        
        private void Awake()
        {
            RandomizeNoiseOffset();
            
            CreateGrid();
        }

        private void RandomizeNoiseOffset()
        {
            float minNoiseOffset = 10000.0f;
            float maxNoiseOffset = -10000.0f;
            _noiseOffset = new(Random.Range(minNoiseOffset, maxNoiseOffset),
                Random.Range(minNoiseOffset, maxNoiseOffset));
        }

        private void GenerateNoiseMap()
        {
            _noise = _noiseGenerator.GenerateMap(
                mapWidth,
                mapHeight,
                noiseScale,
                _noiseOffset.x,
                _noiseOffset.y,
                noiseOctaves,
                noiseAmplitudeChange,
                noiseFrequencyChange
            );

            int width = _noise.GetLength(1);
            int height = _noise.GetLength(0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _noise[y, x] = Mathf.InverseLerp(0.0f, 1.0f, _noise[y, x]);
                }
            }
        }

        private void CreateGrid()
        {
            GenerateNoiseMap();
            
            _tiles = new TileBehaviour[mapHeight, mapWidth];
            
            for (int z = 0; z < mapHeight; z++)
                for (int x = 0; x < mapWidth; x++)
                    _tiles[z,x] = CreateTile(x, z, 0);
        }

        public void RegenerateGrid()
        {
            RandomizeNoiseOffset();
            
            GenerateNoiseMap();

            for (int z = 0; z < mapHeight; z++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    SetTileColor(_tiles[z,x], GetNoiseBasedColor(x, z));
                }
            }
        }

        private TileBehaviour CreateTile(int x, int z, int prefabId)
        {
            Vector3 position = new (
                (x + z % 2 * 0.5f) * TileProperties.InnerRadius * 2, 
                0,
                z * TileProperties.OuterRadius * 1.5f);
            
            TileBehaviour res = Instantiate(tilePrefabs[prefabId], transform, false);
            res.transform.localPosition = position;
            
            SetTileColor(res, GetNoiseBasedColor(x, z));
            
            return res;
        }

        private static readonly Color[] TileColors = { Color.blue, Color.green, Color.gray };
        private Color GetNoiseBasedColor(int x, int z) => _noise[z, x] <= 1.0f - mountainousness ? Color.gray : Color.green; //TileColors[GetNoiseValue(x, z)];

        private int GetNoiseValue(int x, int z)
        {
            //float noise = _noiseGenerator.SampleMap(x, z, noiseScale, _noiseOffset.x, _noiseOffset.y, noiseOctaves, noiseAmplitudeChange, noiseFrequencyChange);

            float noise = _noise[z, x];
            //float noise = Mathf.PerlinNoise(
            //    (float)x / mapWidth * noiseScale + _noiseOffset.x,
            //    (float)z / mapHeight * noiseScale + _noiseOffset.y);
            return Mathf.RoundToInt(noise * (TileColors.Length - 1));
            //return Random.Range(0, 4);
        }

        private void SetTileColor(TileBehaviour tile, Color color)
        {
            tile.GetComponent<HexTileMesh>()?.SetColor(color);
        }
    }
}