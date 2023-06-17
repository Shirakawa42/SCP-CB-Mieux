using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapLoader : MonoBehaviour
{
    private PlayerStats playerStats;
    private MapPrefabs mapPrefabs;

    private Vector2Int previousPlayerTile = new Vector2Int(-1000, -1000);

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        mapPrefabs = GameObject.Find("GameManager").GetComponent<MapPrefabs>();
    }

    void Update()
    {
        if (!Globals.isGameLoaded)
            return;

        Vector2Int playerTile = playerStats.getPlayerTile();

        if (playerTile != previousPlayerTile)
        {
            previousPlayerTile = playerTile;
            LoadTilesAroundPlayer(playerTile);
        }
    }

    private void LoadTilesAroundPlayer(Vector2Int playerTile)
    {
        for (int x = playerTile.x - 3; x <= playerTile.x + 3; x++)
        {
            for (int y = playerTile.y - 3; y <= playerTile.y + 3; y++)
            {
                Vector2Int tilePosition = new Vector2Int(x, y);
                int distanceToPlayer = AstarPathfinding.PathFinding(playerTile, tilePosition, mapPrefabs.mapPrefabsType);

                switch (distanceToPlayer)
                {
                    case 0:
                        mapPrefabs.SetTile(tilePosition, 1);
                        break;
                    case 1:
                        mapPrefabs.SetTile(tilePosition, 1);
                        break;
                    case 2:
                        mapPrefabs.SetTile(tilePosition, 2);
                        break;
                    case 3:
                        mapPrefabs.SetTile(tilePosition, 4);
                        break;
                    default:
                        mapPrefabs.SetTile(tilePosition, 4);
                        break;
                }
            }
        }
    }
}