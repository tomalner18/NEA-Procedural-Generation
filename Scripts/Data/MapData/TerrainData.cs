using System.Collections;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdateableData {

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
	public bool useFlatShading;
	public float uniformScale = 1f;

	public Vector2 mapDimension;

	public float minHeight
	{
		get
		{
			return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
		}
	}
	public float maxHeight
	{
		get
		{
			return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
		}
	}
}
