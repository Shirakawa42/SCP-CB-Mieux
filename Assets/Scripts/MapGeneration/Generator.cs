using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public enum MetroStation { Left, Right, Top, Bottom, Hub };

    private int mapStartX = 500;
    private int mapStartY = 500;
    private int mapEndX = -500;
    private int mapEndY = -500;
    private int roomsToGenerateSave;
    public int roomsToGenerate;
    public int roomsMinDistance;
    public int corridorDensity;
    private MapPrefabs mapPrefabs;
    public Dictionary<MetroStation, Vector2Int> metroStations = new Dictionary<MetroStation, Vector2Int>();
    public Dictionary<Vector2Int, TileType> map = new Dictionary<Vector2Int, TileType>();

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
        Random.InitState(Globals.SEED);
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

    void InstantiateMap()
    {
        foreach (Vector2Int pos in map.Keys)
        {
            if (pos != new Vector2Int(0, 0))
                mapPrefabs.SetTileInstant(pos, 4);
        }
        mapPrefabs.SetTileInstant(Globals.player.GetComponent<PlayerStats>().getPlayerTile(), 1);
    }

    void Start()
    {
        mapPrefabs = GetComponent<MapPrefabs>();
        Globals.player = GameObject.FindGameObjectWithTag("Player");
        Globals.gameManager = this.gameObject;
        roomsToGenerateSave = roomsToGenerate;
        GenerateMap();
        while (RemoveUselessCorridors() || RemoveUselessCorridorsPatterns()) ;
        mapPrefabs.GeneratePrefabs(map);
        InstantiateMap();
        Globals.isGameLoaded = true;
    }
}