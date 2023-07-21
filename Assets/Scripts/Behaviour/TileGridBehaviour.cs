using Base;
using UnityEngine;

namespace Behaviour
{
    public class TileGridBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TileBehaviour[] tilePrefabs;

        private TileBehaviour[,] _tiles;

        public void CreateGrid(int mapHeight, int mapWidth)
        {   
            _tiles = new TileBehaviour[mapHeight, mapWidth];
            
            for (int z = 0; z < mapHeight; z++)
                for (int x = 0; x < mapWidth; x++)
                    _tiles[z,x] = CreateTile(x, z, 0);
        }

        private TileBehaviour CreateTile(int x, int z, int prefabId)
        {
            Vector3 position = new (
                (x + z % 2 * 0.5f) * TileProperties.InnerRadius * 2, 
                0,
                z * TileProperties.OuterRadius * 1.5f);
            
            TileBehaviour res = Instantiate(tilePrefabs[prefabId], transform, false);
            res.transform.localPosition = position;
            
            return res;
        }

        public void SetTileColor(int x, int z, Color color)
        {
            _tiles[z,x].GetComponent<HexTileMesh>()?.SetColor(color);
        }

        public Color GetTileColor(int x, int z)
        {
            return _tiles[z,x].GetComponent<HexTileMesh>().GetColor();
        }
    }
}