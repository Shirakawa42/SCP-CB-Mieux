using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private SortedSet<Node> sortedSet;
    private Dictionary<T, Node> lookup = new Dictionary<T, Node>();
    private class Node : IComparable<Node>
    {
        public T value;
        public float priority;

        public Node(T value, float priority)
        {
            this.value = value;
            this.priority = priority;
        }

        public int CompareTo(Node other)
        {
            return this.priority.CompareTo(other.priority);
        }
    }

    public PriorityQueue()
    {
        this.sortedSet = new SortedSet<Node>();
    }

    public void Enqueue(T value, float priority)
    {
        Node node;
        if (lookup.TryGetValue(value, out node))
        {
            sortedSet.Remove(node);
            node.priority = priority;
        }
        else
        {
            node = new Node(value, priority);
            lookup[value] = node;
        }
        sortedSet.Add(node);
    }

    public T Dequeue()
    {
        var node = sortedSet.Min;
        sortedSet.Remove(node);
        return node.value;
    }

    public int Count
    {
        get { return sortedSet.Count; }
    }

    public bool Contains(T value)
    {
        return lookup.ContainsKey(value);
    }
}

public static class AstarPathfinding
{

    private static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector2Int> GetNeighbours(Vector2Int node)
    {
        return new List<Vector2Int>
        {
            new Vector2Int(node.x - 1, node.y),
            new Vector2Int(node.x + 1, node.y),
            new Vector2Int(node.x, node.y - 1),
            new Vector2Int(node.x, node.y + 1),
        };
    }

    public static int PathFinding(Vector2Int start, Vector2Int end, Dictionary<Vector2Int, PrefabType> map)
    {
        PriorityQueue<Vector2Int> openList = new PriorityQueue<Vector2Int>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> hScore = new Dictionary<Vector2Int, int>();

        gScore[start] = 0;
        hScore[start] = ManhattanDistance(start, end);

        openList.Enqueue(start, hScore[start]);

        while (openList.Count > 0)
        {
            Vector2Int currentNode = openList.Dequeue();

            if (currentNode == end)
            {
                return gScore[currentNode];
            }

            closedList.Add(currentNode);

            List<Vector2Int> neighbours = GetNeighbours(currentNode);

            foreach (var neighbour in neighbours)
            {
                if (!map.ContainsKey(neighbour) || closedList.Contains(neighbour))
                {
                    continue;
                }

                int tentativeGScore = gScore[currentNode] + 1;  // Assuming a uniform movement cost of 1

                // Stop exploring this path if the gScore is more than 3
                if (tentativeGScore > 3)
                {
                    continue;
                }

                if (!openList.Contains(neighbour))
                {
                    openList.Enqueue(neighbour, tentativeGScore + ManhattanDistance(neighbour, end));
                }
                else if (tentativeGScore >= gScore[neighbour])
                {
                    continue;
                }

                gScore[neighbour] = tentativeGScore;
                hScore[neighbour] = tentativeGScore + ManhattanDistance(neighbour, end);
            }
        }

        // If no valid path is found, return a large number
        return 5;
    }
}
