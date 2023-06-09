using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public GameObject inventory;
    public InventorySlot[] slots;
    public InventoryItem head = null;
    public InventoryItem chest = null;
    public InventoryItem hand = null;
    private PlayerStats playerStats;

    public void EquipUnequipSlot(InventoryItem item, int slotIndex)
    {
        InventoryItem part = null;

        if (item.EquipmentType == EquipmentType.Head)
            part = head;
        else if (item.EquipmentType == EquipmentType.Chest)
            part = chest;
        else if (item.EquipmentType == EquipmentType.Hand)
            part = hand;

        if (part != item)
        {
            if (part != null)
            {
                part.OnUnequip(playerStats);
                slots[slotIndex].equippedIndicator.SetActive(false);
                part.slot.equippedIndicator.SetActive(false);
            }
            part = item;
            part.UseOrEquip(playerStats);
            slots[slotIndex].equippedIndicator.SetActive(true);
        }
        else
        {
            part.OnUnequip(playerStats);
            slots[slotIndex].equippedIndicator.SetActive(false);
            part = null;
        }

        if (item.EquipmentType == EquipmentType.Head)
            head = part;
        else if (item.EquipmentType == EquipmentType.Chest)
            chest = part;
        else if (item.EquipmentType == EquipmentType.Hand)
            hand = part;
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Globals.isInventoryOpen)
            {
                inventory.SetActive(false);
                Globals.isInventoryOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                inventory.SetActive(true);
                Globals.isInventoryOpen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void PickUp(GameObject item)
    {
        InventoryItem inventoryItem = item.GetComponent<InventoryItem>();
        if (inventoryItem.MaxStack > 1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.Name == inventoryItem.Name)
                    {
                        slots[i].Amount++;
                        item.gameObject.SetActive(false);
                        item.GetComponent<ItemTriggerScript>().OnTriggerExit(GetComponent<Collider>());
                        Destroy(item);
                        return;
                    }
                }
            }
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = inventoryItem;
                slots[i].Amount = 1;
                slots[i].GetComponent<Image>().sprite = inventoryItem.Icon;
                slots[i].item.transform.localPosition = Vector3.zero;
                slots[i].item.transform.localRotation = Quaternion.identity;
                slots[i].item.gameObject.SetActive(false);
                slots[i].item.slot = slots[i];
                item.GetComponent<ItemTriggerScript>().OnTriggerExit(GetComponent<Collider>());
                return;
            }
        }
    }
}
