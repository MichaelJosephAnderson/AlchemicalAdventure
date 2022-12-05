using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class SetPlayerName : MonoBehaviour
{
   [SerializeField] private TMP_Text _playerName;
   private PhotonView _view;

   private void Start()
   {
       _view = GetComponent<PhotonView>();
       _playerName.text = _view.Owner.NickName;
   }
}
