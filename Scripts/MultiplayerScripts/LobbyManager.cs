using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
   [SerializeField] private TMP_InputField _roomInputField;
   [SerializeField] private GameObject _lobbyPanel;
   [SerializeField] private GameObject _roomPanel;
   [SerializeField] private TMP_Text _roomName;
   
   public RoomItem _roomItemPrefab;
   private List<RoomItem> _roomItemsList = new List<RoomItem>();
   public Transform _contentObject;

   public float _timeBetweenUpdates = 1.5f;
   private float _nextUpdateTime;

   public List<PlayerItem> _playerItemsList = new List<PlayerItem>();
   public PlayerItem _playerItemPrefab;
   public Transform _playerItemParent;

   public GameObject _playButton;

   private void Start()
   {
      PhotonNetwork.JoinLobby();
   }

   private void Update()
   {
      if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
      {
         _playButton.SetActive(true);
      }
      else
      {
         _playButton.SetActive(false);
      }
   }

   public void OnClickCreate()
   {
      if (_roomInputField.text.Length >= 1)
      {
         PhotonNetwork.CreateRoom(_roomInputField.text, new RoomOptions(){MaxPlayers = 2, BroadcastPropsChangeToAll = true});
      }
   }

   public override void OnJoinedRoom()
   {
      _lobbyPanel.SetActive(false);
      _roomPanel.SetActive(true);
      _roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
      UpdatePlayerList();
   }

   public override void OnRoomListUpdate(List<RoomInfo> roomList)
   {
      if (Time.time >= _nextUpdateTime)
      {
         UpdateRoomList(roomList);
         _nextUpdateTime = Time.time + _timeBetweenUpdates;
      }
   }

   private void UpdateRoomList(List<RoomInfo> list)
   {
      foreach (RoomItem item in _roomItemsList)
      {
         Destroy(item.gameObject);
      }
      _roomItemsList.Clear();

      foreach (RoomInfo room in list)
      {
         RoomItem newRoom = Instantiate(_roomItemPrefab, _contentObject);
         newRoom.SetRoomName(room.Name);
         _roomItemsList.Add(newRoom);
      }
   }

   public void JoinRoom(string roomName)
   {
      PhotonNetwork.JoinRoom(roomName);
   }

   public void OnClickLeaveRoom()
   {
      PhotonNetwork.LeaveRoom();
   }

   public override void OnLeftRoom()
   {
      _roomPanel.SetActive(false);
      _lobbyPanel.SetActive(true);
   }

   public override void OnConnectedToMaster()
   {
      PhotonNetwork.JoinLobby();
   }

   void UpdatePlayerList()
   {
      foreach (PlayerItem item in _playerItemsList)
      {
         Destroy(item.gameObject);
      }
      _playerItemsList.Clear();

      if (PhotonNetwork.CurrentRoom == null)
      {
         return;
      }

      foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
      {
         PlayerItem newPlayerItem = Instantiate(_playerItemPrefab, _playerItemParent);
         newPlayerItem.SetPlayerInfo(player.Value);

         if (player.Value == PhotonNetwork.LocalPlayer)
         {
            newPlayerItem.ApplyLocalChanges();
         }

         _playerItemsList.Add(newPlayerItem);
      }
   }

   public override void OnPlayerEnteredRoom(Player newPlayer)
   {
      UpdatePlayerList();
   }

   public override void OnPlayerLeftRoom(Player otherPlayer)
   {
      UpdatePlayerList();
   }

   public void OnClickPlayButton()
   {
      ScenePicker scenePicker = GetComponent<ScenePicker>();
      scenePicker.LoadScene();
   }
}
