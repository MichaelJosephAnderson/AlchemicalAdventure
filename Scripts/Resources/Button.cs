using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Button : MonoBehaviourPunCallbacks
{
    [SerializeField] private ObjectPath _objectPath = null;

    [SerializeField] private MeshRenderer _buttonMeshRenderer = null;
    [SerializeField] private Material _activeMat = null;
    [SerializeField] private Material _deactiveMat = null;

    [SerializeField] private Animator _buttonAnimator = null;
    [SerializeField] private bool _isPressureButton;

    public bool _isActive = false;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (_isPressureButton)
            {
                photonView.RPC("ButtonActivate", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_isPressureButton)
            {
                if (_isActive)
                {
                    photonView.RPC("ButtonDeactivate", RpcTarget.All);
                }
                else 
                {
                    photonView.RPC("ButtonActivate", RpcTarget.All);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (!_isPressureButton)
            {
                photonView.RPC("ButtonAnimateUp", RpcTarget.All);

            }
            else
            {
                photonView.RPC("ButtonDeactivate", RpcTarget.All);
            }

        }
    }

    [PunRPC]
    void ButtonActivate()
    {
        _isActive = true;
        _buttonMeshRenderer.material = _activeMat;
        _buttonAnimator.SetTrigger("ButtonDown");
        _objectPath.StartMovement();
    }
    [PunRPC]
    void ButtonDeactivate()
    {
        _isActive = false;
        _buttonMeshRenderer.material = _deactiveMat;
        _buttonAnimator.SetTrigger("ButtonDown");
        _objectPath.StopMovement();
    }
    [PunRPC]
    void ButtonAnimateUp()
    {
        _buttonAnimator.SetTrigger("ButtonUp");
    }
    
}
