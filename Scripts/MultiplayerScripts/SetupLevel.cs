using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class SetupLevel : MonoBehaviour
{
    public GameObject _objectives;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(_objectives.name, Vector3.zero, Quaternion.identity);
        }
    }
}
