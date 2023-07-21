using Base.Test;
using UnityEngine;

namespace Behaviour.Test
{
    public class NoiseGeneratorBehaviour : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 256)]
        private int noiseMapWidth = 100;
        [SerializeField]
        [Range(1, 256)]
        private int noiseMapHeight = 100;
        [SerializeField]
        [Range(1.0f, 100.0f)]
        private float noiseScale = 1.0f;
        [SerializeField]
        private Vector2 noiseOffset;
        [SerializeField]
        [Range(1, 8)]
        private int noiseOctaves = 1;
        [SerializeField]
        private float noiseAmplitudeChange = 1.0f;
        [SerializeField]
        private float noiseFrequencyChange = 1.0f;

        [SerializeField]
        private Renderer textureRenderer;
        
        private NoiseGenerator _noiseGenerator = new (Mathf.PerlinNoise);
        private Texture2D _noiseMap;

        private void OnValidate()
        {
            float[,] noise = _noiseGenerator.GenerateMap(
                noiseMapWidth,
                noiseMapHeight,
                noiseScale,
                noiseOffset.x,
                noiseOffset.y,
                noiseOctaves,
                noiseAmplitudeChange,
                noiseFrequencyChange
                );

            int width = noise.GetLength(1);
            int height = noise.GetLength(0);
            _noiseMap = new Texture2D(width, height);

            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noise[y, x] = Mathf.InverseLerp(0.0f, 1.0f, noise[y, x]);
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noise[y, x]);
                }
            }
            
            _noiseMap.SetPixels(colorMap);
            _noiseMap.Apply();

            textureRenderer.sharedMaterial.mainTexture = _noiseMap;
            //textureRenderer.transform.localScale = new Vector3(width, 1, height);
        }

        public Texture2D GetNoiseMap() => _noiseMap;
    }
}