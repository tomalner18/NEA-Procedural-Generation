using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bar : MonoBehaviour {

	private float fillAmount;
	[SerializeField]
	private Image content;
	[SerializeField]
	private Text valueText;
	[SerializeField]
	private float lerpSpeed;
	public float MaxValue { get; set; }
	public float Value
	{
		set
		{
			valueText.text = value.ToString();
			fillAmount = ValuetoBar(value, MaxValue);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		UpdateBar();
	}

	private void UpdateBar()
	{
		if (fillAmount != content.fillAmount)
		{
			content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
		}
		
	}
	private float ValuetoBar(float currentValue, float maxValue)
	{
		return Mathf.InverseLerp(0, maxValue, currentValue);
	}
}
