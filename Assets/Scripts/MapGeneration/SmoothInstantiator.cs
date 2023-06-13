using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothInstantiator : MonoBehaviour
{
    private struct PrefabInfos
    {
        public PrefabType type;
        public Vector3 position;
        public bool isDoor;
        public Vector3 doorOffset;
        public int lod;

        public PrefabInfos(PrefabType type, Vector3 position, int lod, bool isDoor = false, Vector3 doorOffset = new Vector3())
        {
            this.type = type;
            this.position = position;
            this.isDoor = isDoor;
            this.doorOffset = doorOffset;
            this.lod = lod;
        }
    }

    private struct ChildrenInfo
    {
        public Vector3 position;
        public GameObject parent;
        public GameObject template;
        public int lod;

        public ChildrenInfo(Vector3 position, GameObject parent, GameObject template, int lod)
        {
            this.position = position;
            this.parent = parent;
            this.template = template;
            this.lod = lod;
        }
    }

    public Dictionary<Vector3, GameObject> instantiatedObjects = new Dictionary<Vector3, GameObject>();
    private Queue<PrefabInfos> instantiatorQueue = new Queue<PrefabInfos>();
    private DoorPool doorPool;

    public void InstantiatePrefab(PrefabType type, Vector3 position, int lod, bool isDoor = false, Vector3 doorOffset = new Vector3())
    {
        instantiatorQueue.Enqueue(new PrefabInfos(type, position, lod, isDoor, doorOffset));
    }

    public void InstantiatePriorityPrefab(PrefabType type, Vector3 position, int lod, bool isDoor = false, Vector3 doorOffset = new Vector3())
    {
        if (!instantiatedObjects.ContainsKey(position))
        {
            GameObject prefab = Instantiate(type.prefab, position, type.rotation);
            instantiatedObjects.Add(position, prefab);
        }
        SetLOD(new PrefabInfos(type, position, lod, isDoor, doorOffset), instantiatedObjects[position]);
    }

    public void AddDoor(PrefabType type, Vector3 position)
    {
        if (!instantiatedObjects.ContainsKey(position))
        {
            GameObject prefab = doorPool.PlaceDoor(position, type.rotation, type.prefab);
            instantiatedObjects.Add(position, prefab);
        }
    }

    public void RemoveDoor(Vector3 position, PrefabType type)
    {
        if (instantiatedObjects.ContainsKey(position))
        {
            doorPool.ReturnDoor(instantiatedObjects[position], type.prefab);
            instantiatedObjects.Remove(position);
        }
    }

    IEnumerator SmoothInstantiatorCoroutine()
    {
        while (true)
        {
            if (instantiatorQueue.Count > 0)
            {
                PrefabInfos prefabInfos = instantiatorQueue.Dequeue();
                if (!instantiatedObjects.ContainsKey(prefabInfos.position))
                {
                    GameObject prefab = Instantiate(prefabInfos.type.prefab, prefabInfos.position, prefabInfos.type.rotation);
                    instantiatedObjects.Add(prefabInfos.position, prefab);
                }
                SetLOD(prefabInfos, instantiatedObjects[prefabInfos.position]);
            }
            yield return null;
        }
    }

    private void SetLOD(PrefabInfos prefabInfos, GameObject parent)
    {
        int currentLOD = GetCurrentLOD(parent);

        parent.SetActive(prefabInfos.lod != 4);
        if (currentLOD > prefabInfos.lod)
        {
            // Activate additional LODs
            for (int i = currentLOD - 1; i >= prefabInfos.lod; i--)
            {
                Transform lodTransform = parent.transform.Find("LOD " + i.ToString());
                if (lodTransform)
                    lodTransform.gameObject.SetActive(true);
            }
        }
        else if (currentLOD < prefabInfos.lod)
        {
            // Deactivate unnecessary LODs
            for (int i = currentLOD; i < prefabInfos.lod; i++)
            {
                Transform lodTransform = parent.transform.Find("LOD " + i.ToString());
                if (lodTransform)
                    lodTransform.gameObject.SetActive(false);
            }
        }
    }

    private int GetCurrentLOD(GameObject parent)
    {
        int currentLOD = int.MaxValue;
        foreach (Transform child in parent.transform)
        {
            if (!child.gameObject.activeSelf)
                continue;
            string[] splitName = child.name.Split(' ');
            if (splitName[0] == "LOD")
            {
                int lodNumber;
                if (int.TryParse(splitName[1], out lodNumber))
                {
                    currentLOD = Mathf.Min(currentLOD, lodNumber);
                }
            }
        }

        return currentLOD == int.MaxValue ? 5 : currentLOD;
    }

    void Start()
    {
        doorPool = GetComponent<DoorPool>();
        StartCoroutine(SmoothInstantiatorCoroutine());
    }
}
