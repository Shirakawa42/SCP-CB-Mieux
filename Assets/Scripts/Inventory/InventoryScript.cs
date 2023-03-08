using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public GameObject inventory;


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
}
