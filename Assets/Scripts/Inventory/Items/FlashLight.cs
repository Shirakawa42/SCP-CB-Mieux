using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : InventoryItem
{
    public override string Name
    {
        get
        {
            return "Flashlight";
        }
    }

    public override string Description
    {
        get
        {
            return "A flashlight";
        }
    }

    public override Sprite Icon
    {
        get
        {
            return Resources.Load<Sprite>("Icons/Flashlight");
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
        GameObject.Find("Player").transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    public override void OnUnequip(PlayerStats playerStats)
    {
        GameObject.Find("Player").transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }
}
