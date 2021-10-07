using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour {

	Quaternion rotation;
	private void Awake()
	{
		transform.rotation = Quaternion.identity;
		transform.Rotate(90, 0, 0);
		rotation = transform.rotation;
		//rotation = transform.rotation;
	}
	private void LateUpdate()
	{
		transform.rotation = rotation;
	}
}
