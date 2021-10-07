using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spawner : MonoBehaviour {

	[SerializeField]
	private GameObject player;
	[SerializeField]
	private GameObject cameraBase;
	[SerializeField]
	private PoissonObject trees;
	[SerializeField]
	private PoissonObject enemies;
	[SerializeField]
	private GameObject[] plants;
	public GameObject Rock;
	MapGen mapGen;
	private float topLeftX, topLeftZ;
	RaycastHit hit;

	public static List<Vector2> points = new List<Vector2>();
	public float iniEnemies;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float r, List<Vector2> points, int[,] grid)
	{
		// Checks whether a point is within the sample area and whether it is within the radius of any others
		if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
		{
			// Finds the cell that the candidate is found in
			// Meaning only surrounding 24 need be checked
			int cellX = (int)(candidate.x / cellSize);
			int cellY = (int)(candidate.y / cellSize);
			// Min and Max prevent errors when candidate is found on the edge
			int startX = Mathf.Max(0, cellX - 2);
			int startY = Mathf.Max(0, cellY - 2);
			int endX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
			int endY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

			for (int x = startX; x <= endX; x++)
			{
				for (int y = startY; y <= endY; y++)
				{ 
					int pointIndex = grid[x, y] - 1;
					if (pointIndex != -1)
					{
						// -1 marks an empty cell as indexing will begin at 0
						float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
						if (sqrDst < r*r)
						{
							// candidate is within the radius of an existing point
							// candidate is therefore rejected
							return false;
						}
					}
				}
			}
			// candidate placement is valid and is accepted
			return true;
		}
		// candidate wasnt in sample area therefore rejected
		return false;
	}


	public List<Vector2> GeneratePoints(float r, Vector2 sampleRegionSize, int k)
	{
		// Generates a list of points where each point isnt within the radius of any others
		// r stores the radius of the points, k is the number of samples to be tested before rejection

		int n = 2; // used to store number of dimensions for sampling in
		float cellSize = r / Mathf.Sqrt(n); // cellsize is defined from radius of points
		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
		List<Vector2> activeList = new List<Vector2>
		{
			new Vector2(UnityEngine.Random.Range(1, sampleRegionSize.x - 1), UnityEngine.Random.Range(1, sampleRegionSize.y - 1))
		};
		while (activeList.Count > 0)
		{
			// iterates while there are still spawn points to be tested
			int activeListIndex = UnityEngine.Random.Range(0, activeList.Count);
			Vector2 activeSpawnPoint = activeList[activeListIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < k; i++)
			{
				// Generates a random position between r and 2r of the spawn point
				Vector2 candidate = activeSpawnPoint + AnnulusOffset(r, 2*r);

				// checks whether a point at the generated position would be valid
				if (IsValid(candidate, sampleRegionSize, cellSize, r, points, grid))
				{
					// if valid then the point is stored and becomes a future spawn point
					points.Add(candidate);
					activeList.Add(candidate);
					// stores point's index at the cell where the point is when
					grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted)
			{
				// no points could be spawned around the active point
				activeList.RemoveAt(activeListIndex);
			}

		}

		return points;
	}

	public void SpawnPoisson(GameObject[] gameObjectArray, float r, int k)
	{
		mapGen = FindObjectOfType<MapGen>();
		// Spawns objects at generated points
		float height = 0f;
		AnimationCurve heightCurve = new AnimationCurve(mapGen.terrainData.meshHeightCurve.keys);
		List<Vector2> points = GeneratePoints(r, mapGen.terrainData.mapDimension * mapGen.mapSectionSize, k);
		topLeftX = ((mapGen.mapSectionSize - 1) * mapGen.terrainData.mapDimension.y) / -2f;
		topLeftZ = ((mapGen.mapSectionSize - 1) * mapGen.terrainData.mapDimension.y) / 2f;

		for (int i = 0; i < points.Count; i++)
		{
			// mesh generation occurs from top left down, thus y values must be done inversely
			float sampleX = topLeftX + points[i].x;
			float sampleY = topLeftZ - points[i].y;
			int floorNoiseX = Mathf.FloorToInt(points[i].x);
			int floorNoiseY = Mathf.FloorToInt(points[i].y);

			Vector3 bottomLeft = new Vector3(floorNoiseX, heightCurve.Evaluate(mapGen.wholeNoiseMap[floorNoiseX, floorNoiseY]), floorNoiseY);
			Vector3 topLeft = new Vector3(floorNoiseX, heightCurve.Evaluate(mapGen.wholeNoiseMap[floorNoiseX, floorNoiseY]), floorNoiseY + 1);
			Vector3 bottomRight = new Vector3(floorNoiseX + 1, heightCurve.Evaluate(mapGen.wholeNoiseMap[floorNoiseX, floorNoiseY]), floorNoiseY);
			Vector3 topRight = new Vector3(floorNoiseX + 1, heightCurve.Evaluate(mapGen.wholeNoiseMap[floorNoiseX, floorNoiseY]), floorNoiseY + 1);


			float current = heightCurve.Evaluate(mapGen.wholeNoiseMap[(int)points[i].x, (int)points[i].y]);

			if (Physics.Raycast(new Vector3(sampleX, 50, sampleY), Vector3.down, out hit, Mathf.Infinity))
			{
				// stores height of terrain point
				height = hit.point.y;
			}
			// only spawns object on grass
			if (current < mapGen.textureData.layers[4].startHeight && current > mapGen.textureData.layers[2].startHeight && hit.collider.tag == "Terrain Section")
			{
				// spawns an object at the generated point and respective height
				GameObject temp = gameObjectArray[UnityEngine.Random.Range(0, gameObjectArray.Length)];
				Instantiate(temp, new Vector3(sampleX, height, sampleY), Quaternion.identity);
				// rotates object randomly around y - axis (rotation swaps y and z)
				temp.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
			}


		}
		points.Clear();
	}

	public void SpawnPlayer()
	{
		float randX = UnityEngine.Random.Range(-mapGen.mapSectionSize * mapGen.terrainData.mapDimension.x / 2, mapGen.mapSectionSize * mapGen.terrainData.mapDimension.x / 2);
		float randY = UnityEngine.Random.Range(-mapGen.mapSectionSize * mapGen.terrainData.mapDimension.y / 2, mapGen.mapSectionSize * mapGen.terrainData.mapDimension.y / 2);
		if (Physics.Raycast(new Vector3(randX, 250, randY), Vector3.down, out hit, Mathf.Infinity))
		{
			if (hit.collider.tag == "Terrain Section" && hit.point.y > mapGen.textureData.layers[1].startHeight)
			{
				player.transform.position = new Vector3(randX, hit.point.y + 50f, randY);
				cameraBase.transform.position = player.transform.position;
			}
		}
	}
	public void Spawn()
	{
		//SpawnPlayer();
		SpawnPoisson(trees.getObjects(), trees.getSpawnRadius(), 30);
		SpawnPoisson(enemies.getObjects(), enemies.getSpawnRadius(), 30);		
	}

	public void SpawnCollectables(Dictionary<string, Item> collectables)
	{
		// Spawns main collectable items on map

		foreach (KeyValuePair<string, Item> item in collectables)
		{
			bool valid = false;
			int count = 0;
			while (!valid)
			{
				Vector2 offset = AnnulusOffset(item.Value.getMinRadius(), item.Value.getMaxRadius() - count);
				// Checks for a collider at random point, stores height of terrain at random point
				if (Physics.Raycast(new Vector3(offset.x, 250, offset.y), Vector3.down, out hit, Mathf.Infinity))
				{
					// Only spawns collectables on reasonably low ground
					if (hit.point.y > mapGen.textureData.layers[2].startHeight * mapGen.terrainData.meshHeightMultiplier && hit.point.y < mapGen.textureData.layers[4].startHeight * mapGen.terrainData.meshHeightMultiplier && hit.collider.tag == "Terrain Section")
					{
						// Spawns collectable
						Instantiate(item.Value.itemObject, new Vector3(offset.x, hit.point.y + 1f, offset.y), Quaternion.identity);
						valid = true;
					}
					count+= 4;
				}
			}
		}
	}

	public void SpawnRandom(GameObject[] gameObjects, float maxHeight, float minHeight, int _count, float heightAdjust)
	{
		bool valid = false;
		int count = 0;
		while (!valid)
		{
			// Generates random coordinate on map to consider
			float randX = UnityEngine.Random.Range(-mapGen.mapSectionSize * mapGen.terrainData.mapDimension.x / 2, mapGen.mapSectionSize * mapGen.terrainData.mapDimension.x / 2);
			float randY = UnityEngine.Random.Range(-mapGen.mapSectionSize * mapGen.terrainData.mapDimension.y / 2, mapGen.mapSectionSize * mapGen.terrainData.mapDimension.y / 2);

			if (Physics.Raycast(new Vector3(randX, 250, randY), Vector3.down, out hit, Mathf.Infinity))
			{ 
				if (hit.point.y > minHeight && hit.point.y < maxHeight && hit.collider.tag == "Terrain Section")
				{
					Instantiate(gameObjects[UnityEngine.Random.Range(0, gameObjects.Length- 1)], new Vector3(randX, hit.point.y + heightAdjust, randY), Quaternion.identity);
					count++;
					if(count == _count)
					{
						valid = true;
					}
				}
			}
		}
	}
	public Vector2 AnnulusOffset(float min, float max)
	{
		float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
		Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		float magnitude = UnityEngine.Random.Range(min, max);
		Vector2 offset = direction * magnitude;
		return offset;
	}
}
[Serializable]
class PoissonObject
{
	[SerializeField]
	private GameObject[] objects;
	[SerializeField]
	private float spawnRadius;
	public PoissonObject()
	{
		
	}
	public GameObject[] getObjects()
	{
		return this.objects;
	}
	public float getSpawnRadius()
	{
		return this.spawnRadius;
	}
}


