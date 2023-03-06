using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private Animator anim;
    private bool isOpen = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OpenCloseDoor()
    {
        if (isOpen)
        {
            anim.SetTrigger("close");
            isOpen = false;
        }
        else if (!isOpen)
        {
            anim.SetTrigger("open");
            isOpen = true;
        }
    }
}
