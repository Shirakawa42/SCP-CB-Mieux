using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAroundPlayer : MonoBehaviour
{
    public Generator generator;
    private Dictionary<Vector2Int, GameObject> mapPrefabs;
    private Dictionary<Vector3, GameObject> allDoors;
    private Vector2Int current_position = new Vector2Int(0, 0);
    private bool first = false;
    private HashSet<Vector2Int> loaded = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> preLoaded = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> currentlyLoaded = new HashSet<Vector2Int>();
    private HashSet<Vector3> currentlyLoadedDoors = new HashSet<Vector3>();
    private HashSet<Vector3> loadedDoors = new HashSet<Vector3>();

    private void Load(Vector2Int pos)
    {
        mapPrefabs[pos].SetActive(true);
        Transform child = mapPrefabs[pos].transform.GetChild(0);
        for (int i = 0; i < child.childCount; i++)
            child.GetChild(i).gameObject.SetActive(true);
        currentlyLoaded.Add(pos);
    }

    private void Unload(Vector2Int pos)
    {
        mapPrefabs[pos].SetActive(false);
    }

    private void PreLoad(Vector2Int pos)
    {
        Transform child = mapPrefabs[pos].transform.GetChild(0);
        for (int i = 0; i < child.childCount; i++)
            child.GetChild(i).gameObject.SetActive(false);
        mapPrefabs[pos].SetActive(true);
        currentlyLoaded.Add(pos);
    }

    private void LoadAround(Vector2Int pos, int depth = 3)
    {
        if (depth == 0 || !mapPrefabs.ContainsKey(pos))
            return;
        if (depth == 1)
        {
            if (!loaded.Contains(pos) && !preLoaded.Contains(pos))
                preLoaded.Add(pos);
        }
        if (depth >= 2)
        {
            if (!loaded.Contains(pos))
                loaded.Add(pos);
            if (preLoaded.Contains(pos))
                preLoaded.Remove(pos);
        }
        LoadAround(new Vector2Int(pos.x + 1, pos.y), depth - 1);
        LoadAround(new Vector2Int(pos.x - 1, pos.y), depth - 1);
        LoadAround(new Vector2Int(pos.x, pos.y + 1), depth - 1);
        LoadAround(new Vector2Int(pos.x, pos.y - 1), depth - 1);
        Vector3[] doorPositions = {
            new Vector3(pos.x * Generator.TILE_SIZE + Generator.TILE_SIZE / 2, 0, pos.y * Generator.TILE_SIZE),
            new Vector3(pos.x * Generator.TILE_SIZE - Generator.TILE_SIZE / 2, 0, pos.y * Generator.TILE_SIZE),
            new Vector3(pos.x * Generator.TILE_SIZE, 0, pos.y * Generator.TILE_SIZE + Generator.TILE_SIZE / 2),
            new Vector3(pos.x * Generator.TILE_SIZE, 0, pos.y * Generator.TILE_SIZE - Generator.TILE_SIZE / 2)
        };
        foreach (Vector3 v in doorPositions)
            if (allDoors.ContainsKey(v) && !loadedDoors.Contains(v))
                loadedDoors.Add(v);
    }

    private void LoadAroundCaller(Vector2Int pos)
    {
        LoadAround(pos);
        foreach (Vector2Int v in currentlyLoaded)
            if (!loaded.Contains(v) && !preLoaded.Contains(v))
                Unload(v);
        foreach (Vector3 v in currentlyLoadedDoors)
            if (!loadedDoors.Contains(v))
                allDoors[v].SetActive(false);
        currentlyLoaded.Clear();
        currentlyLoadedDoors.Clear();
        foreach (Vector2Int v in loaded)
            Load(v);
        foreach (Vector2Int v in preLoaded)
            PreLoad(v);
        foreach (Vector3 v in loadedDoors)
        {
            allDoors[v].SetActive(true);
            currentlyLoadedDoors.Add(v);
        }
        loaded.Clear();
        preLoaded.Clear();
    }

    private void Update()
    {
        Vector2Int new_position = new Vector2Int(Mathf.RoundToInt(transform.position.x / Generator.TILE_SIZE), Mathf.RoundToInt(transform.position.z / Generator.TILE_SIZE));

        if (new_position != current_position && Globals.mapPrefabsLoaded)
        {
            current_position = new_position;
            LoadAroundCaller(current_position);
        }
        if (!first && Globals.mapPrefabsLoaded)
        {
            first = true;
            mapPrefabs = generator.mapPrefabs;
            allDoors = generator.allDoors;
            foreach (Vector2Int v in mapPrefabs.Keys)
                mapPrefabs[v].SetActive(false);
            foreach (Vector3 v in allDoors.Keys)
                allDoors[v].SetActive(false);
            LoadAroundCaller(current_position);
        }
    }
}
