using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public static bool isPaused = false;
    public static bool isInventoryOpen = false;
    

    // inventory dragging
    public static bool isDragging = false;
    public static InventoryItem itemBeingDragged = null;
    public static int amountBeingDragged = 0;
    public static int slotIndexBeingDragged = 0;
    public static bool isEquipped = false;
    //////////////////////
}
