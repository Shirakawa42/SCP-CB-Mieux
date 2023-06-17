using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Animator anim;
    private const float cooldown = .5f;
    private float currentCooldown = 0f;
    private bool isOpen = false;
    private MapPrefabs mapPrefabs;

    public bool isFullyClosed = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        mapPrefabs = GameObject.Find("GameManager").GetComponent<MapPrefabs>();
    }

    void OnEnable()
    {
        if (anim && mapPrefabs)
        {
            anim.ResetTrigger("open");
            anim.ResetTrigger("close");
            if (mapPrefabs.doorPrefabsType[transform.parent.position].isOpenedDoor)
            {
                anim.SetTrigger("open");
                isOpen = true;
            }
            else
            {
                anim.SetTrigger("close");
                isOpen = false;
            }
            currentCooldown = cooldown;
        }
    }

    public void OpenCloseDoor()
    {
        if (currentCooldown <= 0f)
        {
            PrefabType door = mapPrefabs.doorPrefabsType[transform.parent.position];
            currentCooldown = cooldown;
            isFullyClosed = false;
            if (isOpen)
            {
                anim.SetTrigger("close");
                isOpen = false;
                door.isOpenedDoor = false;
            }
            else if (!isOpen)
            {
                anim.SetTrigger("open");
                isOpen = true;
                door.isOpenedDoor = true;
            }
            mapPrefabs.doorPrefabsType[transform.parent.position] = door;
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    void Update()
    {
        if (currentCooldown > 0f)
            currentCooldown -= Time.deltaTime;
        else if (!isFullyClosed && !isOpen)
        {
            isFullyClosed = true;
        }
        //anim.Update(0);
    }
}