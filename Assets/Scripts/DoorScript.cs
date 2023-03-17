using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Animator anim;
    private bool isOpen = false;
    private const float cooldown = 1f;
    private float currentCooldown = 0f;
    private const float autoCloseTime = 3f;
    private float currentAutoCloseTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OpenCloseDoor()
    {
        if (currentCooldown <= 0f)
        {
            currentCooldown = cooldown;
            if (isOpen)
            {
                anim.SetTrigger("close");
                isOpen = false;
            }
            else if (!isOpen)
            {
                anim.SetTrigger("open");
                isOpen = true;
                currentAutoCloseTime = 0f;
            }
        }
    }

    void Update()
    {
        if (currentCooldown > 0f)
            currentCooldown -= Time.deltaTime;
        if (isOpen)
        {
            currentAutoCloseTime += Time.deltaTime;
            if (currentAutoCloseTime >= autoCloseTime)
                OpenCloseDoor();
        }
    }
}
