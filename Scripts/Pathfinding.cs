using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Pathfinder
{
	// stores square root of two so it needn't be calculated 1000s of times
	private const float sqrt2 = 1.41421356237f;
	// stores of the vertices in the map, used as the graph
	private Coordinate[,] graph;
	// stores the height of all the vertices in the world
	private float[,] heightMap;
	private float heuristicCoeff = 0.5f;
	private float costCoeff = 30;    //standard 30
	private float heightCostCoeff = 0.8f;  //standard 0.5
	private float enemyCostCoeff = 30f;
	private float seaLevel;
	float maxtotalcost = 0;
	float averagecost = 0;
	int n = 0;
	public Pathfinder(float[,] heightMap, float seaLevel)
	{
		this.seaLevel = seaLevel;
		this.heightMap = heightMap;

		// creates the graph using heightmap data
		graph = new Coordinate[heightMap.GetLength(0), heightMap.GetLength(1)];
		for (int y = 0; y < heightMap.GetLength(1); y++)
		{
			for (int x = 0; x < heightMap.GetLength(0); x++)
			{
				graph[x,y] = new Coordinate(x, y, heightMap[x, y]);
			}
		}
	}
	public List<Vector3> AStar(Vector2 playerPos, Vector2 target, bool hasEnemyFinder)
	{
		var timer = new System.Diagnostics.Stopwatch();
		timer.Start();

		// finds the node that the goal collectable is located at
		Coordinate goal = graph[(int)target.x + heightMap.GetLength(0) / 2, (int)target.y + heightMap.GetLength(1) / 2];

		if (hasEnemyFinder)
		{
			Enemy enemyPrefab = Object.FindObjectOfType<Enemy>();
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject enemy in enemies)
			{
				// prevent enemies that span the goal from having an influence
				if(target.x >= enemy.transform.position.x - enemyPrefab.getFOVDistance() && target.x <= enemy.transform.position.x + enemyPrefab.getFOVDistance()
					&& target.y >= enemy.transform.position.y - enemyPrefab.getFOVDistance()
					&& target.y <= enemy.transform.position.y + enemyPrefab.getFOVDistance())
				{
					continue;
				}
				// will adjust path by changing costs of nodes near enemies
				// considers any nodes within twice of the enemies fov
				Vector2 enemyPosition = new Vector2((int)enemy.transform.position.x, (int)enemy.transform.position.z);
				for (int y = (int)-enemyPrefab.getFOVDistance(); y <= (int)enemyPrefab.getFOVDistance(); y++)
				{
					for (int x = (int)-enemyPrefab.getFOVDistance(); x <= (int)enemyPrefab.getFOVDistance(); x++)
					{
						float displacement = Mathf.Sqrt(x * x + y * y);		
						graph[(int)(x + enemyPosition.x + heightMap.GetLength(0) / 2)
							, (int)(y + enemyPosition.y + heightMap.GetLength(0) / 2)].setEnemyCost( (enemyPrefab.getFOVDistance() - displacement) / enemyPrefab.getFOVDistance() * enemyCostCoeff + enemyCostCoeff);
						//enemyPrefab.getFOVDistance() / displacement

					}
				}
			}
		}



		// finds the node that the player is located at
		Coordinate current = graph[(int)playerPos.x + heightMap.GetLength(0) / 2, (int)playerPos.y + heightMap.GetLength(1) / 2];

		// creates the frontier, where all nodes to be explored from are stored
		PriorityQueue frontier = new PriorityQueue();

		// stores nodes that have already been visited
		List<Coordinate> used = new List<Coordinate>();

		//adds the player's node to the frontier, as new nodes are found from neighbours of frontier nodes
		frontier.EnQueue(current);

		// marks that the player's node has been visited 
		used.Add(current);
		current.onfrontier = true;
		current.cost = 0;

		while (frontier.Count() > 0)
		{
			// removes the current node from the frontier as it has been explored
			current = frontier.DeQueue();
			if (current == goal)
			{
				// stops algorithm if goal has been reached
				break;
			}
			current.onfrontier = false;
			current.closed = true;
			// double for loop finds all of the neighbouring nodes, maximum 8
			for (int y = Mathf.Max(0, current.y - 1); y < Mathf.Min(heightMap.GetLength(1), current.y + 2); y++)
			{
				for (int x = Mathf.Max(0, current.x - 1); x < Mathf.Min(heightMap.GetLength(0), current.x + 2); x++)
				{
					// creates a temporary copy of a neighbouring node
					Coordinate temp = graph[x, y];
					float newcost = 0f;
					if (current == temp)
					{
						// moves on to next node if the temporary node is the current node
						continue;
					}
					if (!temp.onfrontier && !temp.closed)
					{
						//Node has not been seen before, thus can be added to frontier
						temp.cost = current.cost + MovementCost(current, temp, goal, hasEnemyFinder);
						temp.heuristic = HeuristicCost(temp, goal);
						frontier.EnQueue(temp);
						used.Add(temp);
						temp.onfrontier = true;
						// marks that the temporary node was found by moving from the current node
						temp.cameFrom = current;
						//using assignment returning the value that is assigned
					}
					else if (temp.onfrontier && temp.cost > (newcost = current.cost + MovementCost(current, temp, goal, hasEnemyFinder)))
					{
						//Node has been seen before but there is a faster way of reaching it
						frontier.UpdatePriority(temp, newcost);
						temp.cameFrom = current;
					}
				}
			}
		}
		List<Vector3> path = new List<Vector3>();
		while (current.cameFrom != null)
		{
			path.Add(current.ToVector3());
			current = current.cameFrom;
		}
		path.Reverse();
		foreach (Coordinate c in used)
		{
			c.Reset();
		}
		Debug.Log("Time with Priority Queue is " + timer.ElapsedTicks);
		Debug.Log("Max: " + maxtotalcost);
		Debug.Log("Average: " + averagecost);
		averagecost = 0;
		n = 0;
		timer.Stop();
		return path;
	}

	public float MovementCost(Coordinate from, Coordinate to, Coordinate goal, bool hasEnemyFinder)
	{
		//Calculates the movement cost between two points based on distance/incline
		//Need to not punish downhill as much as uphill
		// Need to punish moving in the FOV of enemies
		float distance = Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) == 1 ? 1 : sqrt2;
		float cost = to.height > from.height ? Mathf.Abs(to.height - from.height) / distance : 0.4f * Mathf.Abs(to.height - from.height) / distance;
		if(hasEnemyFinder)
		{
		cost = cost + (to.getEnemyCost() + from.getEnemyCost()) / 2;
		}
		cost *= costCoeff;
		cost += 1;
		//cost += heightCostCoeff * Mathf.Abs(goal.height - to.height);
		cost += heightCostCoeff * Mathf.Max(goal.height - to.height, 1);
		//cost += heightCostCoeff * to.height * 0.01f;
		//float totalcost = tan * tan * distance * costCoeff; //+ (to.height - goal.height) * heightCostCoeff;
		//maxtotalcost = totalcost > maxtotalcost ? totalcost : maxtotalcost;
		averagecost = (averagecost * n + cost) / (float)(++n);
		return cost;

	}

	public float HeuristicCost(Coordinate from, Coordinate to)
	{
		return from.DistanceTo2D(to) * heuristicCoeff;
	}
}

public class PriorityQueue
{
	List<Coordinate> queue;

	public PriorityQueue()
	{
		queue = new List<Coordinate>();
	}

	public void EnQueue(Coordinate insert)
	{
		// adds a coordinate to the queue, using the priority of the node
		if (queue.Count == 0)
		{
			// if the queue is empty then the node can just be added
			queue.Add(insert);
			return;
		}
		int midpoint = 0;
		// priority is determined by the sum of the cost and heuristic
		int insertPriority = (int)(insert.cost + insert.heuristic);
		int lowerBound = 0;
		int upperBound = queue.Count - 1;
		while (upperBound > lowerBound)
		{
			// binary insertion
			midpoint = (upperBound + lowerBound) / 2;
			if (insertPriority > (int)(queue[midpoint].cost + queue[midpoint].heuristic))
			{
				// removes lower section from consideration for next iteration
				lowerBound = midpoint + 1;
			}
			else if (insertPriority < (int)(queue[midpoint].cost + queue[midpoint].heuristic))
			{
				// removes upper section from consideration for next iteration
				upperBound = midpoint - 1;
			}
			else
			{
				// correct position has been found
				queue.Insert(midpoint + 1, insert);
				return;
			}
		}
		queue.Insert(insertPriority > (int)(queue[lowerBound].cost + queue[lowerBound].heuristic) ? lowerBound + 1 : lowerBound, insert);
	}
	public Coordinate DeQueue()
	{
		// standard dequeue method
		Coordinate front = queue[0];
		queue.RemoveAt(0);
		return front;
	}
	public void UpdatePriority(Coordinate subject, float cost)
	{
		// a faster way of reaching the subject node has been found, so its position in the queue must be changed
		int midpoint = 0;
		int subjectPriority = (int)(subject.cost + subject.heuristic);
		int lowerBound = 0;
		int upperBound = queue.Count - 1;
		while (upperBound > lowerBound)
		{
			// binary search
			midpoint = (upperBound + lowerBound) / 2;
			int midPointPriority = (int)(queue[midpoint].cost + queue[midpoint].heuristic);
			if (midPointPriority == subjectPriority)
			{
				// many nodes may have the same priority - especially from rounding - meaning that the subject could be either side of the calculated midpoint
				int target = CheckSide(subject, midpoint, -1, subjectPriority);
				// target stores the index in the queue where the subject was previously found
				if (target == -1)
				{
					// sign of third parameter of checkside indicates direction
					target = CheckSide(subject, midpoint, 1, subjectPriority);
					if (target == -1)
					{
						if (!queue.Contains(subject))
						{
							throw new System.Exception("It's actually not there");
						}
						throw new System.Exception("Target was not found");
					}
				}
				// removes subject from its previous location
				queue.RemoveAt(target);
				subject.cost = cost;
				// adds subject to the queue, with new position to be found
				EnQueue(subject);
				return;
			}
			else if (subjectPriority > midPointPriority)
			{
				// removes lower section from consideration for next iteration
				lowerBound = midpoint + 1;
			}
			else if (subjectPriority < midPointPriority)
			{
				// removes upper section from next iteration
				upperBound = midpoint - 1;
			}
		}
		// reintroduces subject to queue
		queue.RemoveAt(lowerBound);
		subject.cost = cost;
		EnQueue(subject);
	}



	public void UpdatePriority2(Coordinate subject, float cost)
	{
		queue.Remove(subject);
		subject.cost = cost;
		EnQueue(subject);
	}


	public int CheckSide(Coordinate subject, int midpoint, int direction, int subjectPriority)
	{
		// moves from a point of equal priority with the subject until priority changes
		int target = midpoint;

		while (target >= 0 && target < queue.Count && subjectPriority == (int)(queue[target].cost + queue[target].heuristic))
		{
			if (subject == queue[target])
			{
				return target;
			}
			target += direction;
		}
		// -1 marks that the subject isn't found on the side of the midpoint checked
		return -1;
	}


	public int Count()
	{
		return queue.Count;
	}

}
