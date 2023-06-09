using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Empty, Room, Corridor, Metro };

public static class Globals
{
    public static bool isPaused = false;
    public static bool isInventoryOpen = false;
    public const int TILE_SIZE = 24;
    public const int SEED = 1234;

    // inventory dragging
    public static bool isDragging = false;
    public static InventoryItem itemBeingDragged = null;
    public static int amountBeingDragged = 0;
    public static int slotIndexBeingDragged = 0;
    public static bool isEquipped = false;
    //////////////////////

    public static GameObject player;
    public static GameObject gameManager;
}
