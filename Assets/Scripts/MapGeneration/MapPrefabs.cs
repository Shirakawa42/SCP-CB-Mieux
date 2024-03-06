using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct PrefabType
{
    public GameObject prefab;
    public Quaternion rotation;
    public bool isOpenedDoor;

    public PrefabType(GameObject prefab, Quaternion rotation, bool isOpenedDoor = false)
    {
        this.prefab = prefab;
        this.rotation = rotation;
        this.isOpenedDoor = isOpenedDoor;
    }
}

public class MapPrefabs : MonoBehaviour
{
    private Dictionary<Vector2Int, TileType> map;
    public Dictionary<Vector2Int, PrefabType> mapPrefabsType = new Dictionary<Vector2Int, PrefabType>();
    public Dictionary<Vector3, PrefabType> doorPrefabsType = new Dictionary<Vector3, PrefabType>();
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
    public List<GameObject> doors;
    public GameObject metro;
    public Generator generator;
    public SmoothInstantiator smoothInstantiator;
    public int doorLod = 2;
    

    private void PlaceMetroRooms(Vector2Int pos)
    {
        if (pos == generator.metroStations[Generator.MetroStation.Left])
            mapPrefabsType.Add(pos, new PrefabType(metroRooms[Random.Range(0, metroRooms.Count)], Quaternion.Euler(0, 90, 0)));
        else if (pos == generator.metroStations[Generator.MetroStation.Right])
            mapPrefabsType.Add(pos, new PrefabType(metroRooms[Random.Range(0, metroRooms.Count)], Quaternion.Euler(0, 270, 0)));
        else if (pos == generator.metroStations[Generator.MetroStation.Top])
            mapPrefabsType.Add(pos, new PrefabType(metroRooms[Random.Range(0, metroRooms.Count)], Quaternion.Euler(0, 180, 0)));
        else if (pos == generator.metroStations[Generator.MetroStation.Bottom])
            mapPrefabsType.Add(pos, new PrefabType(metroRooms[Random.Range(0, metroRooms.Count)], Quaternion.Euler(0, 0, 0)));
    }

    private void PlaceMetroCorridors()
    {
        Vector2Int pos = generator.metroStations[Generator.MetroStation.Left];

        while (pos.y < generator.metroStations[Generator.MetroStation.Top].y - 1)
        {
            pos.y++;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 0, 0)));
        }

        pos.y++;
        mapPrefabsType.Add(pos, new PrefabType(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], Quaternion.Euler(0, 0, 0)));

        while (pos.x < generator.metroStations[Generator.MetroStation.Top].x - 1)
        {
            pos.x++;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 90, 0)));
        }

        pos = generator.metroStations[Generator.MetroStation.Top];

        while (pos.x < generator.metroStations[Generator.MetroStation.Right].x - 1)
        {
            pos.x++;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 90, 0)));
        }

        pos.x++;
        mapPrefabsType.Add(pos, new PrefabType(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], Quaternion.Euler(0, 90, 0)));

        while (pos.y > generator.metroStations[Generator.MetroStation.Right].y + 1)
        {
            pos.y--;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 0, 0)));
        }

        pos = generator.metroStations[Generator.MetroStation.Right];

        while (pos.y > generator.metroStations[Generator.MetroStation.Bottom].y + 1)
        {
            pos.y--;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 0, 0)));
        }

        pos.y--;
        mapPrefabsType.Add(pos, new PrefabType(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], Quaternion.Euler(0, 180, 0)));

        while (pos.x > generator.metroStations[Generator.MetroStation.Bottom].x + 1)
        {
            pos.x--;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 90, 0)));
        }

        pos = generator.metroStations[Generator.MetroStation.Bottom];

        while (pos.x > generator.metroStations[Generator.MetroStation.Left].x + 1)
        {
            pos.x--;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 90, 0)));
        }

        pos.x--;
        mapPrefabsType.Add(pos, new PrefabType(metroAngleCorridors[Random.Range(0, metroAngleCorridors.Count)], Quaternion.Euler(0, 270, 0)));

        while (pos.y < generator.metroStations[Generator.MetroStation.Left].y - 1)
        {
            pos.y++;
            mapPrefabsType.Add(pos, new PrefabType(metroStraightCorridors[Random.Range(0, metroStraightCorridors.Count)], Quaternion.Euler(0, 0, 0)));
        }
    }

    private void PlaceMetro()
    {
        Vector2Int pos = generator.metroStations[Generator.MetroStation.Top];

        Instantiate(metro, new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE), Quaternion.Euler(0, 90, 0));
    }

    private void PlaceRooms(Vector2Int pos)
    {
        if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(straightRooms[Random.Range(0, straightRooms.Count)], Quaternion.Euler(0, 90, 0));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(straightRooms[Random.Range(0, straightRooms.Count)], Quaternion.Euler(0, 0, 0));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(angleRooms[Random.Range(0, angleRooms.Count)], Quaternion.Euler(0, 90, 0));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(angleRooms[Random.Range(0, angleRooms.Count)], Quaternion.Euler(0, 270, 0));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(angleRooms[Random.Range(0, angleRooms.Count)], Quaternion.Euler(0, 0, 0));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(angleRooms[Random.Range(0, angleRooms.Count)], Quaternion.Euler(0, 180, 0));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(threeWayRooms[Random.Range(0, threeWayRooms.Count)], Quaternion.Euler(0, 0, 0));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(threeWayRooms[Random.Range(0, threeWayRooms.Count)], Quaternion.Euler(0, 180, 0));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(threeWayRooms[Random.Range(0, threeWayRooms.Count)], Quaternion.Euler(0, 270, 0));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(threeWayRooms[Random.Range(0, threeWayRooms.Count)], Quaternion.Euler(0, 90, 0));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(deadendRooms[Random.Range(0, deadendRooms.Count)], Quaternion.Euler(0, 90, 0));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(deadendRooms[Random.Range(0, deadendRooms.Count)], Quaternion.Euler(0, 270, 0));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(deadendRooms[Random.Range(0, deadendRooms.Count)], Quaternion.Euler(0, 0, 0));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType[pos] = new PrefabType(deadendRooms[Random.Range(0, deadendRooms.Count)], Quaternion.Euler(0, 180, 0));
        }
        else
        {
            mapPrefabsType[pos] = new PrefabType(fourWayRooms[Random.Range(0, fourWayRooms.Count)], Quaternion.Euler(0, 0, 0));
        }

    }

    private bool CheckNeighbour(Vector2Int pos, Vector2Int dir)
    {
        Vector2Int newPos = new Vector2Int(pos.x + dir.x, pos.y + dir.y);

        if (map.ContainsKey(newPos) && map[newPos] != TileType.Empty)
        {
            if (map[newPos] == TileType.Room)
            {
                RoomSpecs room = mapPrefabsType[newPos].prefab.GetComponent<RoomSpecs>();
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
            mapPrefabsType.Add(pos, new PrefabType(straightCorridors[Random.Range(0, straightCorridors.Count)], Quaternion.Euler(0, 90, 0)));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(straightCorridors[Random.Range(0, straightCorridors.Count)], Quaternion.Euler(0, 0, 0)));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(angleCorridors[Random.Range(0, angleCorridors.Count)], Quaternion.Euler(0, 270, 0)));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(angleCorridors[Random.Range(0, angleCorridors.Count)], Quaternion.Euler(0, 90, 0)));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(angleCorridors[Random.Range(0, angleCorridors.Count)], Quaternion.Euler(0, 180, 0)));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(angleCorridors[Random.Range(0, angleCorridors.Count)], Quaternion.Euler(0, 0, 0)));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && !CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], Quaternion.Euler(0, 180, 0)));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && !CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], Quaternion.Euler(0, 0, 0)));
        }
        else if (!CheckNeighbour(pos, new Vector2Int(1, 0))
            && CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], Quaternion.Euler(0, 90, 0)));
        }
        else if (CheckNeighbour(pos, new Vector2Int(1, 0))
            && !CheckNeighbour(pos, new Vector2Int(-1, 0))
            && CheckNeighbour(pos, new Vector2Int(0, 1))
            && CheckNeighbour(pos, new Vector2Int(0, -1)))
        {
            mapPrefabsType.Add(pos, new PrefabType(threeWayCorridors[Random.Range(0, threeWayCorridors.Count)], Quaternion.Euler(0, 270, 0)));
        }
        else
        {
            mapPrefabsType.Add(pos, new PrefabType(fourWayCorridors[Random.Range(0, fourWayCorridors.Count)], Quaternion.Euler(0, 0, 0)));
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
                    if (!doorPrefabsType.ContainsKey(doorPos))
                    {
                        doorPrefabsType.Add(doorPos, new PrefabType(doors[Random.Range(0, doors.Count)], Quaternion.Euler(0, 90, 0)));
                    }
                }
                if (CheckNeighbour(pos, new Vector2Int(-1, 0)))
                {
                    Vector3 doorPos = new Vector3(pos.x * Globals.TILE_SIZE - Globals.TILE_SIZE / 2, 0, pos.y * Globals.TILE_SIZE);
                    if (!doorPrefabsType.ContainsKey(doorPos))
                    {
                        doorPrefabsType.Add(doorPos, new PrefabType(doors[Random.Range(0, doors.Count)], Quaternion.Euler(0, 90, 0)));
                    }
                }
                if (CheckNeighbour(pos, new Vector2Int(0, 1)))
                {
                    Vector3 doorPos = new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE + Globals.TILE_SIZE / 2);
                    if (!doorPrefabsType.ContainsKey(doorPos))
                    {
                        doorPrefabsType.Add(doorPos, new PrefabType(doors[Random.Range(0, doors.Count)], Quaternion.Euler(0, 0, 0)));
                    }
                }
                if (CheckNeighbour(pos, new Vector2Int(0, -1)))
                {
                    Vector3 doorPos = new Vector3(pos.x * Globals.TILE_SIZE, 0, pos.y * Globals.TILE_SIZE - Globals.TILE_SIZE / 2);
                    if (!doorPrefabsType.ContainsKey(doorPos))
                    {
                        doorPrefabsType.Add(doorPos, new PrefabType(doors[Random.Range(0, doors.Count)], Quaternion.Euler(0, 0, 0)));
                    }
                }
            }
        }
    }

    public void SetTile(Vector2Int pos, int lod)
    {
        if (lod <= this.doorLod)
        {
            Vector3[] doorPositions = Utils.getDoorPositionsInTile(pos);
            foreach (Vector3 doorPosition in doorPositions)
            {
                if (doorPrefabsType.ContainsKey(doorPosition))
                    smoothInstantiator.AddDoor(doorPrefabsType[doorPosition], doorPosition);
            }
        }
        else
        {
            Vector3[] doorPositions = Utils.getDoorPositionsInTile(pos);
            foreach (Vector3 doorPosition in doorPositions)
            {
                if (doorPrefabsType.ContainsKey(doorPosition))
                    smoothInstantiator.RemoveDoor(doorPosition, doorPrefabsType[doorPosition]);
            }
        }
        if (mapPrefabsType.ContainsKey(pos))
            smoothInstantiator.InstantiatePrefab(mapPrefabsType[pos], Utils.TileToWorldPosition(pos), lod);
    }

    public void SetTileInstant(Vector2Int pos, int lod)
    {
        if (mapPrefabsType.ContainsKey(pos))
            smoothInstantiator.InstantiatePriorityPrefab(mapPrefabsType[pos], Utils.TileToWorldPosition(pos), lod);
    }

    public void GeneratePrefabs(Dictionary<Vector2Int, TileType> map)
    {
        this.map = map;
        mapPrefabsType.Clear();
        doorPrefabsType.Clear();
        //foreach (Vector2Int pos in map.Keys)
        //    if (map[pos] == TileType.Metro)
        //        PlaceMetroRooms(pos);
        //PlaceMetroCorridors();
        //PlaceMetro();
        foreach (Vector2Int pos in map.Keys)
            if (map[pos] == TileType.Room)
                PlaceRooms(pos);
        foreach (Vector2Int pos in map.Keys)
            if (map[pos] == TileType.Corridor)
                PlaceCorridor(pos);
        PlaceDoors();
        foreach (Vector2Int pos in mapPrefabsType.Keys)
            mapPrefabsType[pos].prefab.SetActive(true);
    }
}
