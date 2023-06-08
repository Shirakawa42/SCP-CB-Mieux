using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public List<GameObject> straightCorridors;
    public List<GameObject> angleCorridors;
    public List<GameObject> threeWayCorridors;
    public List<GameObject> fourWayCorridors;
    public List<GameObject> straightRooms;
    public List<GameObject> angleRooms;
    public List<GameObject> threeWayRooms;
    public List<GameObject> fourWayRooms;
    public List<GameObject> deadendRooms;
    public List<GameObject> metroRooms;
    public List<GameObject> metroStraightCorridors;
    public List<GameObject> metroAngleCorridors;
    public GameObject metro;

    public List<GameObject> doors;
    public int roomsToGenerate;
    public int roomsMinDistance;
    public int corridorDensity;
    
    public enum TileType { Empty, Room, Corridor, Metro };
    private Dictionary<Vector2Int, TileType> map = new Dictionary<Vector2Int, TileType>();
    public Dictionary<Vector2Int, GameObject> mapPrefabs = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector3, GameObject> allDoors = new Dictionary<Vector3, GameObject>();
    private int mapStartX = 500;
    private int mapStartY = 500;
    private int mapEndX = -500;
    private int mapEndY = -500;
    private int roomsToGenerateSave;
    public enum MetroStation { Left, Right, Top, Bottom, Hub };
    public Dictionary<MetroStation, Vector2Int> metroStations = new Dictionary<MetroStation, Vector2Int>();

    private int CountNeighbours(Vector2Int pos)
    {
        int count = 0;
        if (map.ContainsKey(new Vector2Int(pos.x - 1, pos.y)) && map[new Vector2Int(pos.x - 1, pos.y)] != TileType.Empty)
            count++;
        if (map.ContainsKey(new Vector2Int(pos.x + 1, pos.y)) && map[new Vector2Int(pos.x + 1, pos.y)] != TileType.Empty)
            count++;
        if (map.ContainsKey(new Vector2Int(pos.x, pos.y - 1)) && map[new Vector2Int(pos.x, pos.y - 1)] != TileType.Empty)
            count++;
        if (map.ContainsKey(new Vector2Int(pos.x, pos.y + 1)) && map[new Vector2Int(pos.x, pos.y + 1)] != TileType.Empty)
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

    private void getInitialMetroRoomsPosition()
    {
        metroStations.Add(MetroStation.Left, new Vector2Int(-1, 0));
        metroStations.Add(MetroStation.Right, new Vector2Int(1, 0));
        metroStations.Add(MetroStation.Top, new Vector2Int(0, 1));
        metroStations.Add(MetroStation.Bottom, new Vector2Int(0, -1));
        metroStations.Add(MetroStation.Hub, new Vector2Int(0, 0));
    }

    void updateMetroRooms(Vector2Int currentPos)
    {
        if (currentPos.x - 1 < metroStations[MetroStation.Left].x)
        {
            metroStations[MetroStation.Left] = new Vector2Int(currentPos.x - 1, currentPos.y);
        }

        if (currentPos.x + 1 > metroStations[MetroStation.Right].x)
        {
            metroStations[MetroStation.Right] = new Vector2Int(currentPos.x + 1, currentPos.y);
        }

        if (currentPos.y - 1 < metroStations[MetroStation.Bottom].y)
        {
            metroStations[MetroStation.Bottom] = new Vector2Int(currentPos.x, currentPos.y - 1);
        }

        if (currentPos.y + 1 > metroStations[MetroStation.Top].y)
        {
            metroStations[MetroStation.Top] = new Vector2Int(currentPos.x, currentPos.y + 1);
        }
    }

    void addMetroRoomsToMap()
    {
        map.Add(metroStations[MetroStation.Left], TileType.Metro);
        map.Add(metroStations[MetroStation.Right], TileType.Metro);
        map.Add(metroStations[MetroStation.Top], TileType.Metro);
        map.Add(metroStations[MetroStation.Bottom], TileType.Metro);
    }

    private void GenerateMap()
    {
        List<Vector2Int> roomPositions = new List<Vector2Int>();
        Vector2Int currentPos = new Vector2Int(0, 0);
        HashSet<Vector2Int> availablePos = new HashSet<Vector2Int>();

        getInitialMetroRoomsPosition();

        map.Add(currentPos, TileType.Room);
        AddAvailablePositions(new Vector2Int(0, 0), availablePos);

        while (roomsToGenerate > 0 && availablePos.Count > 0)
        {
            currentPos = GetRandomPosition(availablePos);
            while (CountNeighbours(currentPos) > 1)
            {
                if (Random.Range(0, 100) < 50)
                    break;
                availablePos.Remove(currentPos);
                if (availablePos.Count == 0)
                    break;
                currentPos = GetRandomPosition(availablePos);
            }
            if (isNearRoom(currentPos))
            {
                map.Add(currentPos, TileType.Corridor);
                if (currentPos.x < mapStartX)
                    mapStartX = currentPos.x;
                if (currentPos.x > mapEndX)
                    mapEndX = currentPos.x;
                if (currentPos.y < mapStartY)
                    mapStartY = currentPos.y;
                if (currentPos.y > mapEndY)
                    mapEndY = currentPos.y;
            }
            else
            {
                if (Random.Range(0, 100) < corridorDensity)
                {
                    map.Add(currentPos, TileType.Corridor);
                    if (currentPos.x < mapStartX)
                        mapStartX = currentPos.x;
                    if (currentPos.x > mapEndX)
                        mapEndX = currentPos.x;
                    if (currentPos.y < mapStartY)
                        mapStartY = currentPos.y;
                    if (currentPos.y > mapEndY)
                        mapEndY = currentPos.y;
                }
                else
                {
                    map.Add(currentPos, TileType.Room);
                    if (currentPos.x < mapStartX)
                        mapStartX = currentPos.x;
                    if (currentPos.x > mapEndX)
                        mapEndX = currentPos.x;
                    if (currentPos.y < mapStartY)
                        mapStartY = currentPos.y;
                    if (currentPos.y > mapEndY)
                        mapEndY = currentPos.y;
                    roomPositions.Add(currentPos);
                    roomsToGenerate--;
                }
            }

            updateMetroRooms(currentPos);
            AddAvailablePositions(currentPos, availablePos);
            availablePos.Remove(currentPos);
        }

        addMetroRoomsToMap();

        if (roomsToGenerate > 0)
            Debug.Log("Could not generate all rooms");
    }

    private bool RemoveUselessCorridors()
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
            return true;
        return false;
    }

    private bool RemoveUselessCorridorsPatterns()
    {
        bool changed = false;

        for (int x = -50; x <= 50; x++)
        {
            for (int y = -50; y <= 50; y++)
            {
                for (int p = 0; p < Patterns.nbPatterns; p++)
                {
                    bool valid = true;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (Patterns.pattern[p, i, j] == '0')
                            {
                                if (map.ContainsKey(new Vector2Int(x + i, y + j)) && map[new Vector2Int(x + i, y + j)] != TileType.Empty)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            else if (Patterns.pattern[p, i, j] == '1')
                            {
                                if (!map.ContainsKey(new Vector2Int(x + i, y + j)) || map[new Vector2Int(x + i, y + j)] == TileType.Empty)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            else if (Patterns.pattern[p, i, j] == '3')
                            {
                                if (!map.ContainsKey(new Vector2Int(x + i, y + j)) || map[new Vector2Int(x + i, y + j)] != TileType.Corridor)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }
                        if (!valid)
                            break;
                    }
                    if (valid)
                    {
                        changed = true;
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (Patterns.pattern[p, i, j] == '3')
                                {
                                    map[new Vector2Int(x + i, y + j)] = TileType.Empty;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (changed)
            return true;
        return false;
    }

    private bool CheckNeighbour(Vector2Int pos, Vector2Int dir)
    {
        Vector2Int newPos = new Vector2Int(pos.x + dir.x, pos.y + dir.y);

        if (map.ContainsKey(newPos) && map[newPos] != TileType.Empty)
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
            GameObject corridor = Instantiate(straightCorridors[Random.Range(0, straightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(straightCorridors[Random.Range(0, straightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(angleCorridors[Random.Range(0, angleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject corridor = Instantiate(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, corridor);
        }
        else
        {
            GameObject corridor = Instantiate(fourWayCorridors[Random.Range(0, fourWayCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, corridor);
        }
    }

    private void PlaceRooms(Vector2Int pos)
    {
        if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(straightRooms[Random.Range(0, straightRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(straightRooms[Random.Range(0, straightRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(angleRooms[Random.Range(0, angleRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(angleRooms[Random.Range(0, angleRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(angleRooms[Random.Range(0, angleRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(angleRooms[Random.Range(0, angleRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(threeWayRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(threeWayRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(threeWayRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(threeWayRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(deadendRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(deadendRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(deadendRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, room);
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            GameObject room = Instantiate(deadendRooms[Random.Range(0, threeWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, room);
        }
        else
        {
            GameObject room = Instantiate(fourWayRooms[Random.Range(0, fourWayRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, room);
        }
    }

    private void PlaceDoors()
    {
        foreach (Vector2Int pos in map.Keys)
        {
            if (map[pos] == TileType.Corridor)
            {
                if (CheckNeighbour(pos, new Vector2Int(1, 0)))
                {
                    Vector3 doorPos = new Vector3(pos.x * Globals.TILE_SIZE + Globals.TILE_SIZE / 2, 0, pos.y * Globals.TILE_SIZE);
                    if (!allDoors.ContainsKey(doorPos))
                    {
                        allDoors.Add(doorPos, Instantiate(doors[Random.Range(0, doors.Count)], doorPos, Quaternion.Euler(0, 90, 0)));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x + 1, pos.y));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x, pos.y));
                    }
                }
                if (CheckNeighbour(pos, new Vector2Int(-1, 0)))
                {
                    Vector3 doorPos = new Vector3(pos.x * Globals.TILE_SIZE - Globals.TILE_SIZE / 2, 0, pos.y * Globals.TILE_SIZE);
                    if (!allDoors.ContainsKey(doorPos))
                    {
                        allDoors.Add(doorPos, Instantiate(doors[Random.Range(0, doors.Count)], doorPos, Quaternion.Euler(0, 90, 0)));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x - 1, pos.y));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x, pos.y));
                    }
                }
                if (CheckNeighbour(pos, new Vector2Int(0, 1)))
                {
                    Vector3 doorPos = new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE + Globals.TILE_SIZE / 2);
                    if (!allDoors.ContainsKey(doorPos))
                    {
                        allDoors.Add(doorPos, Instantiate(doors[Random.Range(0, doors.Count)], doorPos, Quaternion.Euler(0, 0, 0)));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x, pos.y + 1));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x, pos.y));
                    }
                }
                if (CheckNeighbour(pos, new Vector2Int(0, -1)))
                {
                    Vector3 doorPos = new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE - Globals.TILE_SIZE / 2);
                    if (!allDoors.ContainsKey(doorPos))
                    {
                        allDoors.Add(doorPos, Instantiate(doors[Random.Range(0, doors.Count)], doorPos, Quaternion.Euler(0, 0, 0)));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x, pos.y - 1));
                        allDoors[doorPos].GetComponentInChildren<DoorScript>().linkedTiles.Add(new Vector2Int(pos.x, pos.y));
                    }
                }
            }
        }
    }

    private void PlaceMetroRooms(Vector2Int pos)
    {
        if (pos == metroStations[MetroStation.Left])
        {
            GameObject metroRoom = Instantiate(metroRooms[Random.Range(0, metroRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, metroRoom);
        }
        else if (pos == metroStations[MetroStation.Right])
        {
            GameObject metroRoom = Instantiate(metroRooms[Random.Range(0, metroRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 270, 0));
            mapPrefabs.Add(pos, metroRoom);
        }
        else if (pos == metroStations[MetroStation.Top])
        {
            GameObject metroRoom = Instantiate(metroRooms[Random.Range(0, metroRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 180, 0));
            mapPrefabs.Add(pos, metroRoom);
        }
        else if (pos == metroStations[MetroStation.Bottom])
        {
            GameObject metroRoom = Instantiate(metroRooms[Random.Range(0, metroRooms.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, metroRoom);
        }
    }

    private void PlaceMetroCorridors()
    {
        GameObject metroRoom;
        Vector2Int pos = metroStations[MetroStation.Left];

        while (pos.y < metroStations[MetroStation.Top].y - 1)
        {
            pos.y++;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, metroRoom);
        }

        pos.y++;
        metroRoom = Instantiate(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
        mapPrefabs.Add(pos, metroRoom);

        while (pos.x < metroStations[MetroStation.Top].x - 1)
        {
            pos.x++;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, metroRoom);
        }

        pos = metroStations[MetroStation.Top];

        while (pos.x < metroStations[MetroStation.Right].x - 1)
        {
            pos.x++;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, metroRoom);
        }

        pos.x++;
        metroRoom = Instantiate(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
        mapPrefabs.Add(pos, metroRoom);

        while (pos.y > metroStations[MetroStation.Right].y + 1)
        {
            pos.y--;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, metroRoom);
        }

        pos = metroStations[MetroStation.Right];

        while (pos.y > metroStations[MetroStation.Bottom].y + 1)
        {
            pos.y--;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, metroRoom);
        }

        pos.y--;
        metroRoom = Instantiate(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 180, 0));
        mapPrefabs.Add(pos, metroRoom);

        while (pos.x > metroStations[MetroStation.Bottom].x + 1)
        {
            pos.x--;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, metroRoom);
        }

        pos = metroStations[MetroStation.Bottom];

        while (pos.x > metroStations[MetroStation.Left].x + 1)
        {
            pos.x--;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
            mapPrefabs.Add(pos, metroRoom);
        }

        pos.x--;
        metroRoom = Instantiate(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 270, 0));
        mapPrefabs.Add(pos, metroRoom);

        while (pos.y < metroStations[MetroStation.Left].y - 1)
        {
            pos.y++;
            metroRoom = Instantiate(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 0, 0));
            mapPrefabs.Add(pos, metroRoom);
        }
    }

    private void PlaceMetro()
    {
        Vector2Int pos = metroStations[MetroStation.Top];

        Instantiate(metro, new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
    }

    private void GenerateMapPrefabs()
    {
        foreach (Vector2Int pos in map.Keys)
            if (map[pos] == TileType.Metro)
                PlaceMetroRooms(pos);
        PlaceMetroCorridors();
        PlaceMetro();
        foreach (Vector2Int pos in map.Keys)
            if (map[pos] == TileType.Room)
                PlaceRooms(pos);
        foreach (Vector2Int pos in map.Keys)
            if (map[pos] == TileType.Corridor)
                PlaceCorridor(pos);
        PlaceDoors();
    }

    void Start()
    {
        Globals.player = GameObject.FindGameObjectWithTag("Player");
        Globals.gameManager = this.gameObject;
        roomsToGenerateSave = roomsToGenerate;
        GenerateMap();
        while (RemoveUselessCorridors() || RemoveUselessCorridorsPatterns()) ;
        GenerateMapPrefabs();
        Globals.mapPrefabsLoaded = true;
        foreach (Vector2Int v in mapPrefabs.Keys)
            mapPrefabs[v].SetActive(false);
        foreach (Vector3 v in allDoors.Keys)
            allDoors[v].SetActive(false);
        mapPrefabs[Globals.player.GetComponent<PlayerStats>().getPlayerTile()].GetComponent<PrefabState>().SetLOD(LOD.Full);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Globals.mapPrefabsLoaded = false;
            foreach (GameObject obj in mapPrefabs.Values)
                Destroy(obj);
            foreach (GameObject obj in allDoors.Values)
                Destroy(obj);
            mapPrefabs.Clear();
            map.Clear();
            allDoors.Clear();
            roomsToGenerate = roomsToGenerateSave;
            GenerateMap();
            while (RemoveUselessCorridors() || RemoveUselessCorridorsPatterns()) ;
            GenerateMapPrefabs();
            Globals.mapPrefabsLoaded = true;
            foreach (Vector2Int v in mapPrefabs.Keys)
                mapPrefabs[v].SetActive(false);
            foreach (Vector3 v in allDoors.Keys)
                allDoors[v].SetActive(false);
            mapPrefabs[Globals.player.GetComponent<PlayerStats>().getPlayerTile()].GetComponent<PrefabState>().SetLOD(LOD.Full);
        }
    }
}