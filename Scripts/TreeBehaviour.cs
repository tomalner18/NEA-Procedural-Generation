using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour {

	private GameObject player;
	private Player pPlayer;
	private MapGen mapGen;
	// Use this for initialization
	void Start () {
		pPlayer = FindObjectOfType<Player>();
		player = pPlayer.gameObject;
		mapGen = FindObjectOfType<MapGen>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Vector3.SqrMagnitude(transform.position - player.transform.position) < mapGen.getTreeDistanceThreshold() * mapGen.getTreeDistanceThreshold())
		{
			this.transform.localScale = new Vector3(1, 1, 1);
		}
		else
		{
			this.transform.localScale = new Vector3(0, 0, 0);
		}
	}
}
