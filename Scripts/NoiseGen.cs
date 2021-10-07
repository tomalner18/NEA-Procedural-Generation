
using UnityEngine;
using System.Collections;

public static class NoiseGen
{
    static float maxNoiseHeight = float.MinValue;
    static float minNoiseHeight = float.MaxValue;
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
			// scale must be positive value
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
				// initialising noise constants
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
					// Finds noise coordinate to be sampled
					// Difference between consecutive samples determined by frequency and noise scale
                    float sampleX = (x - (mapWidth/2f) + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - (mapHeight/2f) + octaveOffsets[i].y) / scale * frequency;

					// takes noise value at sample coordinate
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					// adjusts noise value by current amplitude
                    noiseHeight += perlinValue * amplitude;

					// applies persistance and lacunarity between octaves
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
				// finds overall max and min heights
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
				// stores noise value at relevant position in array
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
				// standarises noise heights between min and max heights
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

}
