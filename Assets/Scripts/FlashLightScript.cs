using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightScript : MonoBehaviour
{
    private AudioSource _audioSource;
    private Light   _light;

    void Start()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _light.enabled = !_light.enabled;
        }
    }
}
