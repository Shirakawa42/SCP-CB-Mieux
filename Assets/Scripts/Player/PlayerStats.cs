using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Head,
    Chest,
    Hand,
    All,
    None
}

public class PlayerStats : MonoBehaviour
{
    private int health = 100;
    private int maxHealth = 100;
    private int stamina = 100;
    private int maxStamina = 100;
}
