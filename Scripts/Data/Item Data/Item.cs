using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Item
{
	[SerializeField]
	private string displayName;
	private string description;
	[SerializeField]
	public GameObject itemObject;
	[SerializeField]
	private bool has = false;
	private float minRadius;
	private float maxRadius;
	public Item(string _name, string _description, bool _has, GameObject _itemObject)
	{
		has = _has;
		displayName = _name;
		description = _description;
		itemObject = _itemObject;
	}
	// constructor chaining
	public Item(string _name, string _description, bool _has, GameObject _itemObject, float _minRadius, float _maxRadius) : this(_name, _description, _has, _itemObject)
	{
		minRadius = _minRadius;
		maxRadius = _maxRadius;
	}
	public string getDescription()
	{
		return description;
	}
	public string getDisplayName()
	{
		return displayName;
	}
	public bool getHas()
	{
		return has;
	}
	public void setHas(bool _has)
	{
		this.has = _has;
	}
	public float getMinRadius()
	{
		return minRadius;
	}
	public float getMaxRadius()
	{
		return maxRadius;
	}
}
