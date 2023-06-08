using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LOD { Full, Structure, None }

public class PrefabState : MonoBehaviour
{
    private Vector3[] getDoorPositionsInTile(Vector2Int tile)
    {
        Vector3[] doorPositions = {
            new Vector3(tile.x * Globals.TILE_SIZE + Globals.TILE_SIZE / 2, 0, tile.y * Globals.TILE_SIZE),
            new Vector3(tile.x * Globals.TILE_SIZE - Globals.TILE_SIZE / 2, 0, tile.y * Globals.TILE_SIZE),
            new Vector3(tile.x * Globals.TILE_SIZE, 0, tile.y * Globals.TILE_SIZE + Globals.TILE_SIZE / 2),
            new Vector3(tile.x * Globals.TILE_SIZE, 0, tile.y * Globals.TILE_SIZE - Globals.TILE_SIZE / 2)
        };
        return doorPositions;
    }

    private void EnableDoors(bool enable)
    {
        Dictionary<Vector3, GameObject> allDoors = Globals.gameManager.GetComponent<Generator>().allDoors;
        Vector2Int tile = Utils.WorldPositionToTile(transform.position);
        Vector3[] doorPositions = getDoorPositionsInTile(tile);

        if (enable)
        {
            foreach (Vector3 doorPosition in doorPositions)
            {
                if (allDoors.ContainsKey(doorPosition))
                    allDoors[doorPosition].SetActive(true);
            }
        }
        else
        {
            Vector2Int playerTile = Globals.player.GetComponent<PlayerStats>().getPlayerTile();
            Vector3[] playerDoorPositions = getDoorPositionsInTile(playerTile);
            foreach (Vector3 doorPosition in doorPositions)
            {
                if (allDoors.ContainsKey(doorPosition) && Array.IndexOf(playerDoorPositions, doorPosition) == -1)
                    allDoors[doorPosition].SetActive(false);
            }
        }
    }

    public void SetLOD(LOD lod)
    {
        if (lod == LOD.Full)
            foreach (Transform child in transform.GetChild(0))
                child.gameObject.SetActive(true);
        else if (lod == LOD.Structure)
            foreach (Transform child in transform.GetChild(0))
                child.gameObject.SetActive(false);
        EnableDoors(lod == LOD.Full);
        gameObject.SetActive((lod != LOD.None));
    }
}
