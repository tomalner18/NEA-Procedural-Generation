using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateableData : ScriptableObject {

	public event System.Action OnValuesUpdated;
	public bool autoUpdate;

	#if UNITY_EDITOR

	public void NotifyOfUpdatedValues()
	{
		if (OnValuesUpdated != null)
		{
			UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
			OnValuesUpdated();
		}
	}

	protected virtual void OnValidate()
	{
		if(autoUpdate)
		{
			UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
		}
	}
	#endif
}
