using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

	public void UpdateFromPlayerPrefsFloat(ref float value, string key)
	{
		value = PlayerPrefs.GetFloat(key);
	}

}
