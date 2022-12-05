using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTargetPoint = null;
    [SerializeField]
    private float rotateSpeed;

    private Vector3 rotationOffset;

    private void Start()
    {
        rotationOffset = Vector3.zero;
    }
    void Update()
    {
        transform.position = cameraTargetPoint.transform.position;

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            rotationOffset += new Vector3(0, -90, 0);
        }
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            rotationOffset += new Vector3(0, 90, 0);
        }

        transform.rotation = cameraTargetPoint.transform.rotation * Quaternion.Euler(rotationOffset);
    }
}
