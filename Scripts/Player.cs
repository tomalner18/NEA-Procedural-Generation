using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

	private GameObject inventoryMenu;
	private Manager manager;
	//[SerializeField]
	//private Item compass;
	//public Item getCompass()
	//{
	//	return this.compass;
	//}
	//[SerializeField]
	//private Item enemyFinder;
	//public Item getEnemyFinder()
	//{
	//	return this.enemyFinder;
	//}
	//[SerializeField]
	//private Item pathfinder;
	//public Item getPathfinder()
	//{
	//	return this.pathfinder;
	//}
	//[SerializeField]
	//private Item playerMarker;
	//[SerializeField]
	//public Item getPlayerMarker()
	//{
	//	return this.playerMarker;
	//}
	[SerializeField]
	private GameObject compass;
	[SerializeField]
	private GameObject pathfinder;
	[SerializeField]
	private GameObject enemyFinder;
	[SerializeField]
	private GameObject playerMarker;
	[SerializeField]
	private GameObject goal;
	private MapGen mapGen;
	private Spawner spawner;
	[SerializeField]
	public Dictionary<string, Item> collectables;
	// Use this for initialization
	void Start()
	{
		mapGen = FindObjectOfType<MapGen>();
		spawner = FindObjectOfType<Spawner>();
		float mapConstant = mapGen.mapSectionSize * (mapGen.terrainData.mapDimension.x + mapGen.terrainData.mapDimension.y) / 4;
		// create collectable item instances
		inventoryMenu = GameObject.FindGameObjectWithTag("Inventory Menu");
		manager = FindObjectOfType<Manager>();

		// dictionary to store all collectables, allows for quick lookup on collisions
		collectables = new Dictionary<string, Item>()
		{
			{ "Compass", new Item("Compass", "The compass will integrate itself into your minimap. You should now find it easier to orientate yourself.", false, compass, 0.1f * mapConstant, 0.2f * mapConstant) },
			{ "Pathfinder", new Item("Pathfinder", "When viewing the Pause Menu (P) , the Pathfinder will suggest a route. This will help you to avoid obstacles that may slow you down.", false, pathfinder, 0.6f * mapConstant, 0.75f * mapConstant) },
			{ "Enemy Finder", new Item("Enemy Finder", "When viewing the Pause Menu (P) , the Enemy Finder will highlight the current location of all enemies on the map.", false, enemyFinder, 0.2f * mapConstant, 0.65f * mapConstant) },
			{ "Player Marker", new Item("Player Marker", "When viewing the Pause Menu (P) , the Player Marker will highlight your position on the map.", false, playerMarker, 0.25f * mapConstant, 0.4f * mapConstant) },
			{ "Collectable", new Item("Goal Collectable", "Well Done! You have located the goal collectable, this means that you have won!", false, goal, 0.6f * mapConstant, 0.9f * mapConstant) }
		};
		spawner.SpawnCollectables(collectables);
	}
	private void Awake()
	{
		health.Initialise();
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q))
		{
			health.CurrentValue -= 2;
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			health.CurrentValue += 2;
		}
	}
	public void DamagePlayer(float damage)
	{
		health.CurrentValue -= damage;
	}

	// checks whether a player's collision is with one of the collectables, if so, applys appropriate result
	private void OnTriggerEnter(Collider other)
	{
		string collidedTag = other.gameObject.tag;
		if(collectables.ContainsKey(collidedTag))
		{
			other.gameObject.SetActive(false);
			collectables[collidedTag].setHas(true);
			manager.DisplayPopup(collectables[collidedTag]);
			if(collidedTag == "Collectable")
			{
				Application.Quit();
			}
			else
			{
				health.CurrentValue += 40;
			}
		}
		//if(other.gameObject.CompareTag("Compass"))
		//{
		//	other.gameObject.SetActive(false);
		//	collectables["Compass"].setHas(true);
		//	manager.DisplayPopup(collectables["Compass"]);
		//}
		//if (other.gameObject.CompareTag("Pathfinder"))
		//{
		//	other.gameObject.SetActive(false);
		//	collectables["Pathfinder"].setHas(true);
		//	manager.DisplayPopup(collectables["Pathfinder"]);
		//}
		//if (other.gameObject.CompareTag("EnemyFinder"))
		//{
		//	other.gameObject.SetActive(false);
		//	collectables["Compass"].setHas(true);
		//	manager.DisplayPopup(collectables["Compass"]);
		//}
		//if (other.gameObject.CompareTag("PlayerMarker"))
		//{
		//	other.gameObject.SetActive(false);
		//	playerMarker.setHas(true);
		//	manager.DisplayPopup(playerMarker);

		//}
	}
}
