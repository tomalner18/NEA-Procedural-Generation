using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGen : MonoBehaviour
{

	public enum DrawMode { NoiseMap, Mesh };
	private DrawMode drawMode;
	public enum FalloffFunction { Standard, Sigmoid }
	[SerializeField]
	private FalloffFunction falloffFunction;

	public TerrainData terrainData;
	public NoiseData noiseData;
	public TextureData textureData;

	public Material terrainMaterial;

	[Range(0, 6)]
	public int levelOfDetail;
	public int maxViewDistance;
	public float colliderDistanceThreshold;
	[SerializeField]
	private float treeDistanceThreshold;
	public float getTreeDistanceThreshold()
	{
		return treeDistanceThreshold;
	}


    public float[,] falloffMap, wholeNoiseMap, realMap;
	public int smallCoordinator, globalCoordinator;
	//public int SmallCoordinator { get; set; }


	public bool autoUpdate;
	
	public int mapSectionSize
	{
		get
		{
			if(terrainData.useFlatShading)
			{
				return 95;
			}
			else
			{
				return 239;
			}
		}
	}

    private void Awake()
    {
		textureData.ApplyToMaterial(terrainMaterial);
		PreGenerateWholeMap();
	}

	void OnValuesUpdated()
	{
		if(!Application.isPlaying)
		{
			DrawMapInEditor();
		}
	}

	void OnTextureValuesUpdated ()
	{
		textureData.ApplyToMaterial(terrainMaterial);
	}

	void OnValidate()
	{

		if(terrainData != null)
		{
			terrainData.OnValuesUpdated -= OnValuesUpdated;
			terrainData.OnValuesUpdated += OnValuesUpdated;
		}
		if (noiseData != null)
		{
			noiseData.OnValuesUpdated -= OnValuesUpdated;
			noiseData.OnValuesUpdated += OnValuesUpdated;
		}
		if(textureData != null)
		{
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}
		falloffMap = FalloffGen.GenerateFalloffMap(mapSectionSize, terrainData.mapDimension, falloffFunction);
	}

	public void PreGenerateWholeMap()
	{
		// generates the noisemap for the entire map
		smallCoordinator = -((int)terrainData.mapDimension.x - 1) / 2;
		globalCoordinator = (mapSectionSize) * smallCoordinator;
		// generates falloff map
		falloffMap = FalloffGen.GenerateFalloffMap(mapSectionSize, terrainData.mapDimension, falloffFunction);
		// generates noise map
		wholeNoiseMap = NoiseGen.GenerateNoiseMap(mapSectionSize * (int)terrainData.mapDimension.x + 2, mapSectionSize * (int)terrainData.mapDimension.y + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);
		realMap = new float[wholeNoiseMap.GetLength(0), wholeNoiseMap.GetLength(0)];
		for (int y = 0; y < mapSectionSize * terrainData.mapDimension.y + 2; y++)
		{
			for (int x = 0; x < mapSectionSize * terrainData.mapDimension.x + 2; x++)
			{
				if (noiseData.useFalloff)
				{
					// applies falloff map to noisemap
					wholeNoiseMap[x, y] = Mathf.Clamp01(wholeNoiseMap[x, y] - falloffMap[x, y]);
				}
				// adjusts heights using animation curve
				realMap[x,y] = terrainData.meshHeightCurve.Evaluate(wholeNoiseMap[x, y]);
				// increases heights by multiplyer suitable for gameplay
				realMap[x, y] *= terrainData.meshHeightMultiplier;
			}
		}
	}


	public void DrawMapInEditor()
    {
		PreGenerateWholeMap();
        MapData mapData = GenerateMapData(Vector2.zero);
		//added between
		//float[,] falloffMap = FalloffGen.GenerateFalloffMap(mapSectionSize, new Vector2(1, 1));
		//for (int y = 0; y < mapSectionSize + 2; y++)
		//{
		//	for (int x = 0; x < mapSectionSize + 2; x++)
		//	{
		//		if (noiseData.useFalloff)
		//		{
		//			mapData.heightMap[x, y] = Mathf.Clamp01(mapData.heightMap[x, y] - falloffMap[x, y]);
		//		}
		//		mapData.heightMap[x, y] = terrainData.meshHeightCurve.Evaluate(wholeNoiseMap[x, y]);
		//		mapData.heightMap[x, y] *= terrainData.meshHeightMultiplier;
		//	}
		//}
		////added
		MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGen.TextureFromNoiseMap(mapData.heightMap));        
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(new MeshData(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, terrainData.useFlatShading));
        }
    }



    void Update()
    {
    }

    public MapData GenerateMapData(Vector2 centre)
    {
		// finds the section of noise relevant to section, then applies noise
		// old : float[,] noiseMap = NoiseGen.GenerateNoiseMap(mapSectionSize, mapSectionSize, seed, noiseScale, octaves, persistance, lacunarity, centre + offset);
		float[,] noiseMap = new float[mapSectionSize + 2, mapSectionSize + 2];
		for (int y = 0; y < mapSectionSize + 2; y++)
		{
			for (int x = 0; x < mapSectionSize + 2; x++)
			{
				// converts from negative to positive, allowing noisemap to be indexed
				int noiseFinderX = x + (int)centre.x - globalCoordinator;
				int noiseFinderY = y - (int)centre.y - globalCoordinator;
				noiseMap[x, y] = wholeNoiseMap[noiseFinderX, noiseFinderY];
			}
		}
		// applies heights to terrain texture
		textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
		// returns noise-map for mesh generation
        return new MapData(noiseMap);
    }


}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;

    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
