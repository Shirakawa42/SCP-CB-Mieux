using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFlashlight : MonoBehaviour
{
    public Transform main_camera;
    public float speed = 5.0f;
    
    void Update()
    {
        transform.LookAt(main_camera.forward);
    }
}
