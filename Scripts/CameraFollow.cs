using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	[SerializeField]
	private float cameraMoveSpeed = 120.0f;
	[SerializeField]
	public float targetingMoveSpeed = 0.5f;
	[SerializeField]
	private GameObject cameraFollowObject;
	[SerializeField]
	private float clampAngle = 80.0f;
	private float mouseX, mouseY;
	private float finalInputX, finalInputZ;
	private float smoothX, smoothY;
	private float rotationY, rotationX = 0;
	private CameraState cameraState = CameraState.Free;
	public enum CameraState
	{
		Target, Free
	}

	private void Awake()
	{
		PlayerPrefsManager manager = FindObjectOfType<PlayerPrefsManager>();
		//manager.UpdateFromPlayerPrefsFloat(ref cameraMoveSpeed, "Camera Move Speed");
		//manager.UpdateFromPlayerPrefsFloat(ref cameraMoveSpeed, "Camera Targeting Speed");
	}
	// Use this for initialization
	void Start () {
		Vector3 rotation = transform.localRotation.eulerAngles;
		rotationY = rotation.y;
		rotationX = rotation.x;
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetAxis("Target") > 0.01)
		{
			TargetingRotation();
		}
		else
		{
			float controllerInputX = Input.GetAxis("RightStickHorizontal");
			float controllerInputZ = Input.GetAxis("RightStickVertical");
			mouseX = Input.GetAxis("Mouse X");
			mouseY = Input.GetAxis("Mouse Y");
			finalInputX = controllerInputX + mouseX;
			finalInputZ = controllerInputZ + mouseY;

			rotationY += finalInputX * cameraMoveSpeed * Time.deltaTime;
			rotationX += finalInputZ * cameraMoveSpeed * Time.deltaTime;
			rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);
			transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
		}
	}

	void LateUpdate()
	{

		UpdateCameraBase();
	}
	void UpdateCameraBase()
	{
		Transform target = cameraFollowObject.transform;
		float step = cameraMoveSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, target.position, step);
	}
	void TargetingRotation()
	{
		Transform target = cameraFollowObject.transform;
		rotationY = target.rotation.eulerAngles.y;
		rotationX = target.rotation.eulerAngles.x;
		float step = targetingMoveSpeed * Time.deltaTime;
		transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, step);		
	}
}
