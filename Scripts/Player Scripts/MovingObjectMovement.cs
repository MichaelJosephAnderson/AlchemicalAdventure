using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectMovement : MonoBehaviour
{
    private BasicMovement basicMovement;
    private PlayerMovementController playerMovementController;

    private void OnEnable()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        basicMovement = GetComponent<BasicMovement>();
    }

    public void MoveWithObject() 
    {
        if (Physics.Raycast(basicMovement.destination, Vector3.down, out RaycastHit hitInfo, 10f))
        {
            basicMovement.destination = hitInfo.transform.position + new Vector3(0, 1f, 0);
        }
    }

}
