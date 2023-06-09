using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public InventoryScript inventory;
    public GameObject dummy;

    public void OnDrop()
    {
        if (Globals.isDragging)
        {
            if (Globals.isEquipped)
                inventory.EquipUnequipSlot(Globals.itemBeingDragged, Globals.slotIndexBeingDragged);
            Globals.itemBeingDragged.Prefab.SetActive(true);
            Globals.itemBeingDragged.Prefab.transform.position = new Vector3(inventory.transform.position.x, inventory.transform.position.y - .9f, inventory.transform.position.z);
            Globals.itemBeingDragged.Prefab.transform.rotation =  inventory.transform.rotation;
            Globals.itemBeingDragged.slot = null;
            Globals.isDragging = false;
            Globals.itemBeingDragged = null;
            Globals.amountBeingDragged = 0;
            Globals.slotIndexBeingDragged = 0;
            Globals.isEquipped = false;
            dummy.SetActive(false);
        }
    }
}
