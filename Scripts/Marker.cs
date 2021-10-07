using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour {

	public Transform player;
	[SerializeField]
	private Transform mainCamera;
	private Vector3 desiredEuler;
	private Vector3 playerEuler;
	private Vector3 cameraEuler;
	
	// Update is called once per frame
	void LateUpdate () {
		cameraEuler = mainCamera.rotation.eulerAngles;
		playerEuler = player.rotation.eulerAngles;

		desiredEuler = new Vector3(0, 0, 57.5f) - new Vector3(0, 0, playerEuler.y) + new Vector3(0,0, cameraEuler.y);
		transform.rotation = Quaternion.Euler(desiredEuler);
		
	}
}
