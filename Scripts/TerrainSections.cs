using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainSections : MonoBehaviour
{
	const float scale = 1f;
	float topLeftX;
	float topLeftZ;
	[SerializeField]
	private Transform viewer;
	[SerializeField]
	private Material mapMaterial;
	[SerializeField]
	private static Vector2 viewerPosition;
	static MapGen mapGen;
	static Spawner spawner;
	private Vector2 mapDimension;
	int sectionSize;

	// Allows a terrain section to be found from its coordinate
	public Dictionary<Vector2, TerrainSection> terrainSectionDictionary = new Dictionary<Vector2, TerrainSection>();

	void Start()
    {
        mapGen = FindObjectOfType<MapGen>();
		spawner = FindObjectOfType<Spawner>();
        sectionSize = mapGen.mapSectionSize - 1;
		topLeftX = sectionSize / -2f;
		topLeftZ = sectionSize / -2f;
        mapDimension = mapGen.terrainData.mapDimension;
		CreateMapSections();
		
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
		// Checks each terrain section
        for (int y = -(int)((mapDimension.y - 1) / 2); y <= (int)((mapDimension.y - 1) / 2); y++)
        {
			for (int x = -(int)((mapDimension.x - 1) / 2); x <= (int)((mapDimension.x - 1) / 2); x++)
			{

				Vector2 viewedSectionCoordinate = new Vector2(x, y);
				terrainSectionDictionary[(viewedSectionCoordinate)].UpdateTerrainSection();
			}
        }
    }
	public void CreateMapSections()
	{
		// creates map sections centred around the origin
		for (int y = mapGen.smallCoordinator; y <= -mapGen.smallCoordinator; y++)
		{
			for (int x = mapGen.smallCoordinator; x <= -mapGen.smallCoordinator; x++)
			{
				Vector2 sectionCoordinate = new Vector2(x, y);
				// Creates section and adds it to dictionary
				terrainSectionDictionary.Add(sectionCoordinate, new TerrainSection(sectionCoordinate, sectionSize, transform, mapMaterial));
			}
		}
		// Once terrain is generated, the objects can be spawned
		spawner.Spawn();
	}

	public class TerrainSection
	{
		static int instanceCounter = 0;
		GameObject meshObject;
		Vector2 position;
		Bounds bounds;


		MeshRenderer meshRenderer;
		public MeshFilter meshFilter;
		public MeshCollider meshCollider;
		public MeshData meshData;
		public MapData mapData;



		public TerrainSection(Vector2 coord, int size, Transform parent, Material material)
		{
			// Constructor instantiates a new terrain section with appropriate mesh

			// Finds global position from coordinate and section size
			position = coord * size;
			bounds = new Bounds(position, Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x, 0, position.y);
			meshObject = new GameObject("Terrain Section " + ++instanceCounter)
			{
				// Tag can be used for object spawning when checking colliders
				tag = "Terrain Section"
			};
			// Adds required mesh components
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider = meshObject.AddComponent<MeshCollider>();

			meshRenderer.material = material;
			meshObject.transform.position = positionV3 * scale;
			meshObject.transform.parent = parent;
			meshObject.transform.localScale = Vector3.one * scale;

			// Gets respective noise values for terrain section
			mapData = mapGen.GenerateMapData(position);

			// Creates section mesh from noise values
			meshData = new MeshData(mapData.heightMap, mapGen.terrainData.meshHeightMultiplier, mapGen.terrainData.meshHeightCurve, mapGen.terrainData.useFlatShading);

			// Applies mesh
			meshFilter.mesh = meshData.CreateMesh();
			meshCollider.sharedMesh = meshFilter.mesh;
		}



		public void UpdateTerrainSection()
		{
			// Only render terrain section if it is within the view threshold
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			bool visible = viewerDstFromNearestEdge <= mapGen.maxViewDistance;		
			if(!visible)
			{
				meshObject.layer = 15;
			}
			else
			{
				meshObject.layer = 1;
			}
		}

		public void SetVisible(bool visible)
		{
			meshObject.SetActive(visible);
		}

		public bool IsVisible()
		{
			return meshObject.activeSelf;
		}

	}
}