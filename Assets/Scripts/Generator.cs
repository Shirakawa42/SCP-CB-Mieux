using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<GameObject> straightCorridors;
    public List<GameObject> angleCorridors;
    public List<GameObject> threeWayCorridors;
    public List<GameObject> fourWayCorridors;
    public List<GameObject> rooms;
    public List<GameObject> doors;
    public int roomsToGenerate;
    public int roomsMinDistance;
    public int corridorDensity;
    private const int TILE_SIZE = 24;
    public enum TileType { Empty, Room, Corridor };
    private Dictionary<Vector2Int, TileType> map = new Dictionary<Vector2Int, TileType>();
    private Dictionary<Vector2Int, GameObject> mapPrefabs = new Dictionary<Vector2Int, GameObject>();

    private int CountNeighbours(Vector2Int pos)
    {
        int count = 0;
        if (map.ContainsKey(new Vector2Int(pos.x - 1, pos.y)))
            count++;
        if (map.ContainsKey(new Vector2Int(pos.x + 1, pos.y)))
            count++;
        if (map.ContainsKey(new Vector2Int(pos.x, pos.y - 1)))
            count++;
        if (map.ContainsKey(new Vector2Int(pos.x, pos.y + 1)))
            count++;
        return count;
    }

    private void AddAvailablePositions(Vector2Int pos, HashSet<Vector2Int> availablePos)
    {
        if (!map.ContainsKey(new Vector2Int(pos.x - 1, pos.y)) && CountNeighbours(new Vector2Int(pos.x - 1, pos.y)) < 2)
            availablePos.Add(new Vector2Int(pos.x - 1, pos.y));
        if (!map.ContainsKey(new Vector2Int(pos.x + 1, pos.y)) && CountNeighbours(new Vector2Int(pos.x + 1, pos.y)) < 2)
            availablePos.Add(new Vector2Int(pos.x + 1, pos.y));
        if (!map.ContainsKey(new Vector2Int(pos.x, pos.y - 1)) && CountNeighbours(new Vector2Int(pos.x, pos.y - 1)) < 2)
            availablePos.Add(new Vector2Int(pos.x, pos.y - 1));
        if (!map.ContainsKey(new Vector2Int(pos.x, pos.y + 1)) && CountNeighbours(new Vector2Int(pos.x, pos.y + 1)) < 2)
            availablePos.Add(new Vector2Int(pos.x, pos.y + 1));
    }

    private Vector2Int GetRandomPosition(HashSet<Vector2Int> availablePos)
    {
        int index = Random.Range(0, availablePos.Count);
        int i = 0;
        foreach (Vector2Int p in availablePos)
        {
            if (i == index)
                return p;
            i++;
        }
        return new Vector2Int(0, 0);
    }

    private bool isNearRoom(Vector2Int pos)
    {
        for (int x = -roomsMinDistance; x <= roomsMinDistance; x++)
        {
            for (int y = -roomsMinDistance; y <= roomsMinDistance; y++)
            {
                if (map.ContainsKey(new Vector2Int(pos.x + x, pos.y + y)))
                {
                    if (map[new Vector2Int(pos.x + x, pos.y + y)] == TileType.Room)
                        return true;
                }
            }
        }
        return false;
    }

    private void GenerateMap()
    {
        List<Vector2Int> roomPositions = new List<Vector2Int>();
        Vector2Int currentPos = new Vector2Int(0, 0);
        HashSet<Vector2Int> availablePos = new HashSet<Vector2Int>();
        map.Add(currentPos, TileType.Room);
        AddAvailablePositions(new Vector2Int(0, 0), availablePos);
        while (roomsToGenerate > 0 && availablePos.Count > 0)
        {
            currentPos = GetRandomPosition(availablePos);
            while (CountNeighbours(currentPos) > 1)
            {
                availablePos.Remove(currentPos);
                if (availablePos.Count == 0)
                    break;
                currentPos = GetRandomPosition(availablePos);
            }
            if (isNearRoom(currentPos))
            {
                map.Add(currentPos, TileType.Corridor);
            }
            else
            {
                if (Random.Range(0, 100) < corridorDensity)
                    map.Add(currentPos, TileType.Corridor);
                else
                {
                    map.Add(currentPos, TileType.Room);
                    roomPositions.Add(currentPos);
                    roomsToGenerate--;
                }
            }
            AddAvailablePositions(currentPos, availablePos);
            availablePos.Remove(currentPos);
        }
        if (roomsToGenerate > 0)
            Debug.Log("Could not generate all rooms");
    }

    private void RemoveUselessCorridors()
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (Vector2Int pos in map.Keys)
        {
            if (map[pos] == TileType.Corridor)
            {
                if (CountNeighbours(pos) <= 1)
                    toRemove.Add(pos);
            }
        }
        foreach (Vector2Int pos in toRemove)
            map.Remove(pos);
        if (toRemove.Count > 0)
            RemoveUselessCorridors();
    }

    private bool CheckNeighbour(Vector2Int pos, Vector2Int dir)
    {
        Vector2Int newPos = new Vector2Int(pos.x + dir.x, pos.y + dir.y);

        if (map.ContainsKey(newPos))
        {
            if (map[newPos] == TileType.Room)
            {
                RoomSpecs room = mapPrefabs[newPos].GetComponent<RoomSpecs>();
                if (pos.x > newPos.x)
                {
                    if (room.xplus)
                        return true;
                }
                else if (pos.x < newPos.x)
                {
                    if (room.xminus)
                        return true;
                }
                else if (pos.y > newPos.y)
                {
                    if (room.yplus)
                        return true;
                }
                else if (pos.y < newPos.y)
                {
                    if (room.yminus)
                        return true;
                }
            }
            return true;
        }
        return false;
    }

    private void PlaceCorridor(Vector2Int pos)
    {
        if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(straightCorridors[Random.Range(0, straightCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE),Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(straightCorridors[Random.Range(0, straightCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else
        {
            GameObject corridor = Instantiate(fourWayCorridors[Random.Range(0, fourWayCorridors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
    }

    private struct doorPlaced
    {
        public Vector2Int pos;
        public short direction;
    }

    private void PlaceDoors()
    {
        HashSet<doorPlaced> doorPositions = new HashSet<doorPlaced>();

        foreach (Vector2Int pos in map.Keys)
        {
            if (map[pos] == TileType.Corridor)
            {
                if (CheckNeighbour(pos, new Vector2Int(1, 0)) && !doorPositions.Contains(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 3 }))
                {
                    Instantiate(doors[Random.Range(0, doors.Count)], new Vector3(pos.x * TILE_SIZE + TILE_SIZE / 2, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 90, 0));
                    doorPositions.Add(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 0 });
                }
                if (CheckNeighbour(pos, new Vector2Int(-1, 0)) && !doorPositions.Contains(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 3 }))
                {
                    Instantiate(doors[Random.Range(0, doors.Count)], new Vector3(pos.x * TILE_SIZE - TILE_SIZE / 2, 0, pos.y * TILE_SIZE), Quaternion.Euler(0, 90, 0));
                    doorPositions.Add(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 1 });
                }
                if (CheckNeighbour(pos, new Vector2Int(0, 1)) && !doorPositions.Contains(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 3 }))
                {
                    Instantiate(doors[Random.Range(0, doors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE + TILE_SIZE / 2), Quaternion.Euler(0, 0, 0));
                    doorPositions.Add(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 2 });
                }
                if (CheckNeighbour(pos, new Vector2Int(0, -1)) && !doorPositions.Contains(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 3 }))
                {
                    Instantiate(doors[Random.Range(0, doors.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE - TILE_SIZE / 2), Quaternion.Euler(0, 0, 0));
                    doorPositions.Add(new doorPlaced { pos = pos + new Vector2Int(1, 0), direction = 3 });
                }
            }
        }
    }

    private void GenerateMapPrefabs()
    {
        foreach (Vector2Int pos in map.Keys)
        {
            if (map[pos] == TileType.Room)
            {
                GameObject room = Instantiate(rooms[Random.Range(0, rooms.Count)], new Vector3(pos.x * TILE_SIZE, 0, pos.y * TILE_SIZE), Quaternion.identity);
                mapPrefabs.Add(pos, room);
            }
        }
        foreach (Vector2Int pos in map.Keys)
            if (map[pos] == TileType.Corridor)
                PlaceCorridor(pos);
        PlaceDoors();
    }

    void Start()
    {
        GenerateMap();
        RemoveUselessCorridors();
        GenerateMapPrefabs();
    }
}