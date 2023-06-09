using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsPool : MonoBehaviour
{
    private Dictionary<GameObject, List<GameObject>> pool = new Dictionary<GameObject, List<GameObject>>();
    public int maxPrefabOnHoldPerType;

    public Dictionary<Vector3, GameObject> instantiatedPrefabs = new Dictionary<Vector3, GameObject>();

    public GameObject Create(PrefabType prefabType, Vector3 pos, bool isDoor, Vector3 doorOffset = new Vector3())
    {
        if (instantiatedPrefabs.ContainsKey(pos))
            return instantiatedPrefabs[pos];
        if (!pool.ContainsKey(prefabType.prefab) || pool[prefabType.prefab].Count == 0)
            instantiatedPrefabs[pos] = Instantiate(prefabType.prefab, pos, prefabType.rotation);
        else
        {
            GameObject obj = pool[prefabType.prefab][0];
            pool[prefabType.prefab].RemoveAt(0);
            obj.transform.position = pos;
            obj.transform.rotation = prefabType.rotation;
            obj.SetActive(true);
            instantiatedPrefabs[pos] = obj;
        }
        if (isDoor)
        {
            instantiatedPrefabs[pos].GetComponentInChildren<DoorScript>().linkedTiles.Clear();
            instantiatedPrefabs[pos].GetComponentInChildren<DoorScript>().linkedTiles.Add(Utils.WorldPositionToTile(pos - doorOffset));
            instantiatedPrefabs[pos].GetComponentInChildren<DoorScript>().linkedTiles.Add(Utils.WorldPositionToTile(pos + doorOffset));
        }
        return instantiatedPrefabs[pos];
    }

    public void Destroy(Vector3 pos, PrefabType prefabType)
    {
        if (!instantiatedPrefabs.ContainsKey(pos))
            return;
        instantiatedPrefabs[pos].SetActive(false);
        if (!pool.ContainsKey(prefabType.prefab))
            pool[prefabType.prefab] = new List<GameObject>();
        pool[prefabType.prefab].Add(instantiatedPrefabs[pos]);
        instantiatedPrefabs.Remove(pos);
        if (pool[prefabType.prefab].Count > maxPrefabOnHoldPerType)
        {
            Destroy(pool[prefabType.prefab][0]);
            pool[prefabType.prefab].RemoveAt(0);
        }
    }
}
