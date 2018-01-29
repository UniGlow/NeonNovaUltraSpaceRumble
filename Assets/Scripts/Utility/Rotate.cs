using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simplay rotates the attached GameObject in every update.
/// </summary>
public class Rotate : MonoBehaviour
{

    [Tooltip("Rotate speed in angle per second")]
    public float rotateSpeed = 6f;
    


    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}

