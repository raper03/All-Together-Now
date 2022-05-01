using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// </summary>
/// Quick camera movement script, to move the camera.
/// <summary>

public class CameraMovement : MonoBehaviour
{
    
    public float speed;

    // Update is called once per frame
    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");        

        transform.Translate(new Vector3(xAxisValue * speed, 0.0f, zAxisValue * speed));

    }
}
