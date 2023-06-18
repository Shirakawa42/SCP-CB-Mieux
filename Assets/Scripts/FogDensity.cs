using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDensity : MonoBehaviour
{
    public float StartFogDistance = 15f;
    public float EndFogDistance = 20f;

    void Update()
    {
        float cameraDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        
        if (cameraDistance > StartFogDistance)
        {
            float fogDensity = Mathf.InverseLerp(StartFogDistance, EndFogDistance, cameraDistance);
            RenderSettings.fogDensity = fogDensity;
        }
        else
        {
            RenderSettings.fogDensity = 0;
        }
    }
}
