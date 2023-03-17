using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : HandTarget
{
    public const float clickCooldown = 1f;
    private float currentClickCooldown = 0f;
    private bool isEnable = true;
    private const float autoCloseTime = 3f;
    private float currentAutoCloseTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.transform.GetComponent<PlayerHandScript>().AddTarget(transform.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.transform.GetComponent<PlayerHandScript>().RemoveTarget(transform.gameObject);
    }

    public override void Click(PlayerHandScript other)
    {
        if (currentClickCooldown <= 0f)
        {
            currentClickCooldown = clickCooldown;
            transform.parent.GetComponent<DoorScript>().OpenCloseDoor();
            other.RemoveTarget(transform.gameObject);
            transform.GetComponent<BoxCollider>().enabled = false;
            isEnable = false;
            transform.GetComponent<AudioSource>().Play();
        }
    }

    void Update()
    {
        if (currentClickCooldown > 0f)
        {
            currentClickCooldown -= Time.deltaTime;
        }
        else if (currentClickCooldown <= 0f && !isEnable)
        {
            transform.GetComponent<BoxCollider>().enabled = true;
            isEnable = true;
        }
    }
}
