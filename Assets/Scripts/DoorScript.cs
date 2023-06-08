using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Animator anim;
    private const float cooldown = .6f;
    private float currentCooldown = 0f;
    private const float autoCloseTime = 3f;
    private float currentAutoCloseTime = 0f;
    private bool isOpen = false;
    private MapPrefabs mapPrefabs;

    public List<Vector2Int> linkedTiles = new List<Vector2Int>();
    public bool isFullyClosed = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        mapPrefabs = GameObject.Find("GameManager").GetComponent<MapPrefabs>();
    }

    private void EnableLinkedTiles(bool enable)
    {
        foreach (Vector2Int tile in linkedTiles)
        {
            if (tile != Globals.player.GetComponent<PlayerStats>().getPlayerTile())
                if (enable)
                    mapPrefabs.EnableTile(tile);
                else
                    mapPrefabs.DisableTile(tile);
        }
    }

    public void OpenCloseDoor()
    {
        if (currentCooldown <= 0f)
        {
            currentCooldown = cooldown;
            isFullyClosed = false;
            EnableLinkedTiles(true);
            if (isOpen)
            {
                anim.SetTrigger("close");
                isOpen = false;
            }
            else if (!isOpen)
            {
                anim.SetTrigger("open");
                isOpen = true;
                currentAutoCloseTime = 0f;
            }
        }
    }

    void Update()
    {
        if (currentCooldown > 0f)
            currentCooldown -= Time.deltaTime;
        else if (!isFullyClosed && !isOpen)
        {
            isFullyClosed = true;
            EnableLinkedTiles(false);
        }
        if (isOpen)
        {
            currentAutoCloseTime += Time.deltaTime;
            if (currentAutoCloseTime >= autoCloseTime)
                OpenCloseDoor();
        }
    }
}