using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
	[SerializeField]
	private Transform Player;
	[SerializeField]
	private Transform MainCamera;
	[SerializeField]
	private GameObject North;

	private void LateUpdate()
	{
		Vector3 newPosition = Player.position;
		newPosition.y = transform.position.y;
		transform.position = newPosition;
		// rotates minimap with camera
		transform.rotation = Quaternion.Euler(90f, MainCamera.eulerAngles.y, 0);
		// rotates north marker around minimap circle
		North.transform.localPosition = new Vector3(Mathf.Sin(-MainCamera.eulerAngles.y * Mathf.PI / 180) * 50, Mathf.Cos(MainCamera.eulerAngles.y * Mathf.PI / 180) * 50, 0);
	}
}
