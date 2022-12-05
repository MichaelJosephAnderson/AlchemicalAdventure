using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomItem : MonoBehaviour
{
   [SerializeField] private TMP_Text _roomName;
   private LobbyManager _manager;

   private void Start()
   {
      _manager = FindObjectOfType<LobbyManager>();
   }

   public void SetRoomName(string roomName)
   {
      _roomName.text = roomName;
   }

   public void OnClickItem()
   {
      _manager.JoinRoom(_roomName.text);
   }
}
