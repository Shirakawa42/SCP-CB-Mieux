using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem : MonoBehaviour
{
    abstract public string Name { get; }
    abstract public string Description { get; }
    abstract public Sprite Icon { get; }
    abstract public GameObject Prefab { get; }
    abstract public EquipmentType EquipmentType { get; }
    abstract public int MaxStack { get; }
    abstract public InventorySlot slot { get; set; }

    abstract public void UseOrEquip(PlayerStats playerStats);
    abstract public void OnUnequip(PlayerStats playerStats);
}
