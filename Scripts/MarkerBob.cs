using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBob : MonoBehaviour {

	private Vector3 ini;
	private float totalTime = 0;
	[SerializeField]
	private float speed;
	[SerializeField]
	private float amplitude;

	public void MoveCosine ()
	{
		transform.localPosition = new Vector3(ini.x, 10 + ini.y + amplitude * Mathf.Abs(Mathf.Cos(totalTime * speed)), ini.z);
	}
	public void SetPosition(Vector3 playerPos, int mapSize)
	{
		float percentX = playerPos.x / mapSize;
		float percentY = playerPos.z / mapSize;
		transform.localPosition = new Vector3(percentX * 360, percentY * 360, 0);
		ini = transform.localPosition;
		StartCoroutine("Oscillate", .05f);
	}
	IEnumerator Oscillate(float delay)
	{
		while(gameObject.activeInHierarchy)
		{
			yield return new WaitForSecondsRealtime(delay);
			totalTime += delay;
			MoveCosine();
		}
	}
}
