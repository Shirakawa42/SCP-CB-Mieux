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
    private Queue<ChildrenInfo> childrenQueue = new Queue<ChildrenInfo>();

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

    IEnumerator SmoothChildrenInstantiatorCoroutine()
    {
        while (true)
        {
            if (childrenQueue.Count > 0)
            {
                ChildrenInfo childrenInfo = childrenQueue.Dequeue();
                if (instantiatedObjects.ContainsKey(childrenInfo.position))
                {
                    Transform LODTransform = Instantiate(new GameObject("LOD " + childrenInfo.lod.ToString()), instantiatedObjects[childrenInfo.position].transform).transform;
                    LODTransform.localPosition = Vector3.zero;
                    Debug.Log(childrenInfo.template.transform.childCount);
                    foreach (Transform child in childrenInfo.template.transform)
                    {
                        GameObject newChild = Instantiate(child.gameObject, LODTransform);
                        newChild.transform.localPosition = child.localPosition;
                        newChild.name = child.name;
                        yield return new WaitForSeconds(0.1f);
                    }
                }
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
                if (lodTransform == null)
                {
                    // Instantiate missing LOD from the original prefab
                    Transform originalLodTransform = prefabInfos.type.prefab.transform.Find("LOD " + i.ToString());
                    if (originalLodTransform != null)
                        childrenQueue.Enqueue(new ChildrenInfo(prefabInfos.position, parent, originalLodTransform.gameObject, i));
                }
            }
        }
        else if (currentLOD < prefabInfos.lod)
        {
            // Deactivate unnecessary LODs
            for (int i = currentLOD; i < prefabInfos.lod; i++)
            {
                Transform lodTransform = parent.transform.Find("LOD " + i.ToString());
                if (lodTransform != null)
                {
                    Destroy(lodTransform.gameObject);
                }
            }
        }
    }

    private int GetCurrentLOD(GameObject parent)
    {
        int currentLOD = int.MaxValue;
        foreach (Transform child in parent.transform)
        {
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
        StartCoroutine(SmoothInstantiatorCoroutine());
        StartCoroutine(SmoothChildrenInstantiatorCoroutine());
    }
}
