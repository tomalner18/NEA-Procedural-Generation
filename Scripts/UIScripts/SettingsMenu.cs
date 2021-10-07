using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {

	public void SetCameraDistance(float value)
	{
		PlayerPrefs.SetFloat("Camera Distance", value);
	}
	public void SetCameraMoveSpeed(float value)
	{
		PlayerPrefs.SetFloat("Camera Move Speed", value);
	}
	public void SetCameraTargetingSpeed(float value)
	{
		PlayerPrefs.SetFloat("Camera Targeting Speed", value);
	}
}
