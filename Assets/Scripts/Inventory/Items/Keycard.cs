using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : InventoryItem
{
    public int accessLevel;

    public override string Name
    {
        get
        {
            return "Level " + accessLevel + " Keycard";
        }
    }

    public override string Description
    {
        get
        {
            return "A keycard that can be equipped to open doors";
        }
    }

    public override Sprite Icon
    {
        get
        {
            return Resources.Load<Sprite>("Icons/Keycard"+accessLevel+"Icon");
        }
    }

    public override GameObject Prefab
    {
        get
        {
            return gameObject;
        }
    }

    public override EquipmentType EquipmentType
    {
        get
        {
            return EquipmentType.Hand;
        }
    }

    public override int MaxStack
    {
        get
        {
            return 1;
        }
    }

    public override InventorySlot slot { get; set; }

    public override void UseOrEquip(PlayerStats playerStats)
    {
        Debug.Log("Equiped Keycard");
        playerStats.currentAccessLevel = accessLevel;
    }

    public override void OnUnequip(PlayerStats playerStats)
    {
        Debug.Log("Unequiped Keycard");
        playerStats.currentAccessLevel = 0;
    }
}
