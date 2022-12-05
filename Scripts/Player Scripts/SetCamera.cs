using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private AudioListener _audioListener;
    [SerializeField] private RenderTexture _specatorCamera;
    private PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        
        if (!_view.IsMine)
        {
            _camera.targetTexture = _specatorCamera;
            _audioListener.enabled = false;
            //_camera.SetActive(false);
        }
    }
    
}