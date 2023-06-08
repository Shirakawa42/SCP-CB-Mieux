using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTriggerScript : HandTarget
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.GetComponent<PlayerHandScript>().AddTarget(this.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.GetComponent<PlayerHandScript>().RemoveTarget(this.gameObject);
    }

    public override void Click(PlayerHandScript other)
    {
        other.GetComponent<InventoryScript>().PickUp(this.gameObject);
    }
}
