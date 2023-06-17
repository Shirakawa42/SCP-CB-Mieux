using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDetailsSpawn : MonoBehaviour
{
    public float probability;
    public int maxCount;

    private int count = 0;

    void Start()
    {
        Random.InitState(Globals.SEED + Utils.WorldPositionToTile(transform.position).GetHashCode());

        List<GameObject> childToDestroy = new List<GameObject>();
        foreach (Transform child in transform)
            if (Random.value > probability)
            {
                child.gameObject.SetActive(true);
                if (++count >= maxCount)
                    break;
            }
            else
                childToDestroy.Add(child.gameObject);
        foreach (GameObject child in childToDestroy)
            Destroy(child);
    }
}
