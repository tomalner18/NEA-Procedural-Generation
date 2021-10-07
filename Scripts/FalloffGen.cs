using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGen{


    public static float[,] GenerateFalloffMap(int size, Vector2 mapDimension, MapGen.FalloffFunction falloffFunction)
    {

		float[,] map = new float[size * (int)mapDimension.x + 2, size * (int)mapDimension.y + 2];
        for (int i = 0; i < size* mapDimension.x + 2; i++)
        {
            for (int j = 0; j < size * mapDimension.y + 2 ; j++)
            {
                float x = j / (float)(size * mapDimension.y) * 2 - 1;
                float y = i / (float)(size * mapDimension.x) * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				if (falloffFunction == MapGen.FalloffFunction.Sigmoid)
				{
					map[i, j] = EvaluateSigmoid(value, mapDimension);
				}
				else if(falloffFunction == MapGen.FalloffFunction.Standard)
				{
					map[i, j] = EvaluateStandard(value, mapDimension);
				}
            }
        }
        return map;
    }
    public static float EvaluateStandard(float value, Vector2 mapDimension)
    {
        float a = 3f;
        float b = (float)mapDimension.x;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
    public static float EvaluateSigmoid(float value, Vector2 mapDimension)
    {
        float a = -4 * (float)mapDimension.x;
        return  Mathf.Exp(6 * value - 1) / (Mathf.Exp(6 * value - 1) + Mathf.Pow(value,a));
    }
}
