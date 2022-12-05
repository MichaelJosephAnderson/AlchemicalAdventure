using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    private PlayerMaster playerMaster;

    private void OnEnable()
    {
        SpawnPlayers._OnPlayerSpawn += SetInitialReferences;
    }

    private void OnDisable()
    {
        SpawnPlayers._OnPlayerSpawn -= SetInitialReferences;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (other.GetComponentInParent<PhotonView>().IsMine)
            {
                playerMaster.CallEventPlayerGetsHit();
            }
        }
    }

    void SetInitialReferences() 
    {
        playerMaster = FindObjectOfType<PlayerMaster>();
    }
}
