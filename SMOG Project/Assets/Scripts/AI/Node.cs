using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
	public bool walkable; // Checks for wheter the "object" is walkable or not
	public Vector3 worldPosition; // Point in the world that node represents
	public int gridX;
	public int gridY;
	public int movementPenalty;

	public int gCost; // Distance from  starting node
	public int hCost; // Distance from end node
	public Node parent;
	int heapIndex;

	//Constructor for Node
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		movementPenalty = _penalty;
	}

	public int fCost // The lowes f cost is the most favorable node to search
	{
		get
		{
			return gCost + hCost;
		}
	}

	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
