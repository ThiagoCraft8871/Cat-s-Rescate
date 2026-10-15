using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform[] backgrounds;
    public float[] parallaxSpeeds;

    private float cameraHorizontalPosition;
    private float previousCameraHorizontalPosition;
    void Start()
    {
        
    }

    void Update()
    {
        cameraHorizontalPosition = Camera.main.transform.position.x;

        if (cameraHorizontalPosition != previousCameraHorizontalPosition)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                float parallax = (previousCameraHorizontalPosition - cameraHorizontalPosition) * parallaxSpeeds[i];
                backgrounds[i].position += new Vector3(parallax, 0f, 0f);
            }
            previousCameraHorizontalPosition = cameraHorizontalPosition;
        }
    }
}
