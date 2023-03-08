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
    abstract public bool Equiped { get; }

    abstract public void UseOrEquip();
    abstract public void OnUnequip();
}
