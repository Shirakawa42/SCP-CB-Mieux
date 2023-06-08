using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2Int WorldPositionToTile(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x + Globals.TILE_SIZE / 2) / Globals.TILE_SIZE);
        int y = Mathf.FloorToInt((position.z + Globals.TILE_SIZE / 2) / Globals.TILE_SIZE);

        return new Vector2Int(x, y);
    }
}
