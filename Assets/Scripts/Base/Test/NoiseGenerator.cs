using System;

namespace Base.Test
{
    public class NoiseGenerator
    {
        private readonly Func<float, float, float> _noiseSampleFunction;

        public NoiseGenerator(Func<float, float, float> noiseSampleFunction)
        {
            _noiseSampleFunction = noiseSampleFunction;
        }

        public float[,] GenerateMap(int width, int height, float noiseScale, float noiseOffsetX, float noiseOffsetY, int noiseOctaves, float amplitudeChange, float frequencyChange)
        {
            float[,] result = new float[height, width];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    result[y, x] = SampleMap(x, y, noiseScale, noiseOffsetX, noiseOffsetY, noiseOctaves, amplitudeChange, frequencyChange);

            return result;
        }

        public float SampleMap(int x, int y, float noiseScale, float noiseOffsetX, float noiseOffsetY, int noiseOctaves, float amplitudeChange, float frequencyChange)
        {
            float amplitude = 1.0f;
            float frequency = 1.0f;
            float result = 0.0f;
                    
            for (int o = 0; o < noiseOctaves; o++)
            {
                float sample = _noiseSampleFunction(
                    x / noiseScale * frequency + noiseOffsetX,
                    y / noiseScale * frequency + noiseOffsetY);

                result += sample * amplitude;
                amplitude *= amplitudeChange;
                frequency *= frequencyChange;
            }

            return result;
            //return Math.Clamp(result, 0.0f, 1.0f);
        }
    }
}