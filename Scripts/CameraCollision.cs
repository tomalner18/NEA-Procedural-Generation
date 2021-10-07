using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
	[SerializeField]
	private float minDistance = 0.2f;
	[SerializeField]
	private float maxDistance = 5.0f;
	[SerializeField]
	private float smooth = 10.0f;
	[SerializeField]
	public float targetSmooth = 1000.0f;
	private Vector3 dollyDir;
	private float distance; //the standard distance of the camera from the player


	void Awake()
	{
		PlayerPrefsManager manager = FindObjectOfType<PlayerPrefsManager>();
		//manager.UpdateFromPlayerPrefsFloat(ref maxDistance, "Camera View Distance");
		dollyDir = transform.localPosition.normalized; // distance from camera base, normalised to have a magnitude of 1
		distance = transform.localPosition.magnitude; // distance * dollyDir = distance of the camera from the base
	}

	// Update is called once per frame
	void Update()
	{
		SetFreePosition();
	}
	public void SetFreePosition()
	{
		Vector3 desiredCameraPosition = transform.parent.TransformPoint(dollyDir * maxDistance); // the desired position is the closest it can be to the camera base when moving by the max distance
		RaycastHit hit;

		if (Physics.Linecast(transform.parent.position, desiredCameraPosition, out hit))
		{
			distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance); // returns the distance from the camera to the hit object reduced by 10%
		}
		else
		{
			distance = maxDistance;
		}
		transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
	}
}
