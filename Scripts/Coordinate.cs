//Prevents issue of reference equality being used in int arrays
using UnityEngine;

public class Coordinate
{
	//In order to keep this immutable (the value does not change once it is set)
	//Coordinates should only be created with the constructors and should not have
	//their values changed after.
	public int x;
	public int y;
	// stores the height of the terrain at coordinate
	public float height;
	public bool onfrontier;
	public bool closed;
	public float cost;
	public float heuristic;
	private float enemyCost;
	// marks the node that this was reached from, such that the path can be backtracked
	public Coordinate cameFrom;
	public Coordinate(int x, int y, float height)
	{
		this.x = x;
		this.y = y;
		this.height = height;
		onfrontier = false;
		closed = false;
		cost = float.MaxValue;
		//Indicator value that heuristic has not been set
		heuristic = -1f;
		cameFrom = null;
		enemyCost = 0;
	}
	public Coordinate(Vector2 pos, float height) : this((int)pos.x, (int)pos.y, height) { }
	public void Reset()
	{
		// Resets all values as pathfinding may occur multiple times
		onfrontier = false;
		closed = false;
		cost = float.MaxValue;
		heuristic = -1;
		cameFrom = null;
		enemyCost = 0;
	}
	public float getEnemyCost()
	{
		return enemyCost;
	}
	public void setEnemyCost(float enemyCost)
	{
		this.enemyCost = enemyCost;
	}
	public float DistanceTo(Coordinate p, float[,] heightMap)
	{
		// Finds the distance between two nodes in 3D
		int xSum = x - p.x;
		int ySum = y - p.y;
		float zSum = heightMap[x, y] - heightMap[p.x, p.y];
		return Mathf.Sqrt(xSum * xSum + ySum * ySum + zSum * zSum);
	}
	public float DistanceTo2D(Coordinate p)
	{
		// Finds the planar distance between two nodes
		int xSum = x - p.x;
		int ySum = y - p.y;
		return Mathf.Sqrt(xSum * xSum + ySum * ySum);
	}
	public Vector3 ToVector3()
	{
		return new Vector3(x, height, y);
	}
}
