using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	[SerializeField]
	private UnityEngine.UI.Button btn;

	public void Start()
	{
		PreSelect();
	}
	public void PreSelect()
	{
		btn.Select();
	}
}
