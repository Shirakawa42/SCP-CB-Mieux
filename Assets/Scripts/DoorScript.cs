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
    private Generator generator;

    public List<Vector2Int> linkedTiles = new List<Vector2Int>();
    public bool isFullyClosed = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        generator = GameObject.Find("GameManager").GetComponent<Generator>();
    }

    private void SetLinkedTilesLOD(LOD lod)
    {
        foreach (Vector2Int tile in linkedTiles)
        {
            if (tile != Globals.player.GetComponent<PlayerStats>().getPlayerTile())
                generator.mapPrefabs[tile].GetComponent<PrefabState>().SetLOD(lod);
        }
    }

    public void OpenCloseDoor()
    {
        if (currentCooldown <= 0f)
        {
            currentCooldown = cooldown;
            isFullyClosed = false;
            SetLinkedTilesLOD(LOD.Full);
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
            SetLinkedTilesLOD(LOD.None);
        }
        if (isOpen)
        {
            currentAutoCloseTime += Time.deltaTime;
            if (currentAutoCloseTime >= autoCloseTime)
                OpenCloseDoor();
        }
    }
}