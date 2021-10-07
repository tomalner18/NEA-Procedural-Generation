using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapPathManager : MonoBehaviour{

	private GameObject collectable;
	private MapGen mapGen;
	[HideInInspector]
	public Texture2D minimapOverlay;
	[SerializeField]
	private RawImage rawImage;
	private List<Vector3> path;
	public Pathfinder pathfinder;


	private void Awake()
	{
		//mapGen = FindObjectOfType<MapGen>();
		//pathfinder = new Pathfinder(mapGen.realMap, mapGen.textureData.layers[1].startHeight * mapGen.terrainData.meshHeightMultiplier);
		//minimapOverlay = new Texture2D(720, 720);
		//var rawImage = this.GetComponent<RawImage>();
		//if (rawImage == null)
		//{
		//	Debug.Log("Raw Image null");
		//}
		//this.GetComponent<RawImage>().texture = minimapOverlay;
		//collectable = GameObject.FindGameObjectWithTag("Collectable");
	}

	private void Start()
	{
		mapGen = FindObjectOfType<MapGen>();
		pathfinder = new Pathfinder(mapGen.realMap, mapGen.textureData.layers[1].startHeight * mapGen.terrainData.meshHeightMultiplier);
		//minimapOverlay = new Texture2D(720, 720);
		minimapOverlay = new Texture2D((int)mapGen.terrainData.mapDimension.x * mapGen.mapSectionSize, (int)mapGen.terrainData.mapDimension.y * mapGen.mapSectionSize);
		rawImage.texture = minimapOverlay;
	}
	public void UpdatePath(GameObject player, bool hasPathfinder, bool hasEnemyFinder, GameObject[] enemies)
	{
		collectable = GameObject.FindGameObjectWithTag("Collectable");
		// removes previous values on texture
		ClearMap();
		if(hasPathfinder)
		{
			// finds and draws path if player has item
			path = pathfinder.AStar(new Vector2(player.transform.position.x, player.transform.position.z),
new Vector2(collectable.transform.position.x, collectable.transform.position.z), hasEnemyFinder);
			DrawPath();
		}
		// applys path to minimap
		minimapOverlay.Apply();
	}
	public void DrawPath()
	{
		foreach (Vector3 p in path)
		{
			for (int y = (int)p.z - 1; y <= (int)p.z + 1; y++)
			{
				for (int x = (int)p.x - 1; x <= (int)p.x + 1; x++)
				{
					// colours all nodes that are found on the path
					// colours the immediate neighbours of each node for thickness
					minimapOverlay.SetPixel(x, y, Color.cyan);
				}
			}
			//minimapOverlay.SetPixel((int)p.x, (int)p.z, Color.red);
		}
	}
	public void ClearMap()
	{
		for (int i = 0; i < mapGen.terrainData.mapDimension.x * mapGen.mapSectionSize; i++)
		{
			for (int j = 0; j < mapGen.terrainData.mapDimension.y * mapGen.mapSectionSize; j++)
			{
				// resets texture
				minimapOverlay.SetPixel(i, j, Color.clear);
			}
		}
	}	
}
