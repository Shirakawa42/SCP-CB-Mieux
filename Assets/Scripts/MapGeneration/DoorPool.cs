using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPool : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> doorPools = new Dictionary<GameObject, Queue<GameObject>>();
    private int nbDefaultDoors;

    public GameObject PlaceDoor(Vector3 position, Quaternion rotation, GameObject type)
    {
        if (!doorPools.ContainsKey(type))
        {
            doorPools.Add(type, new Queue<GameObject>());
            for (int i = 0; i < nbDefaultDoors; i++) {
                GameObject door = Instantiate(type, new Vector3(1000f, 1000f, 1000f), rotation);
                door.SetActive(false);
                doorPools[type].Enqueue(door);
            }
        }
        if (doorPools[type].Count == 0)
            return Instantiate(type, position, rotation);
        else
        {
            GameObject door = doorPools[type].Dequeue();
            door.transform.position = position;
            door.transform.rotation = rotation;
            door.SetActive(true);
            return door;
        }
    }

    public void ReturnDoor(GameObject door, GameObject type)
    {
        door.SetActive(false);
        if (!doorPools.ContainsKey(type))
            doorPools.Add(type, new Queue<GameObject>());
        doorPools[type].Enqueue(door);
    }


    void Start()
    {
        nbDefaultDoors = 16 / GetComponent<MapPrefabs>().doors.Count;   
    }
}
