using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviourPunCallbacks
{
   public TMP_Text _playerName;

   public Image _backgroundImage;
   public Color _highlightColor;
   public GameObject _leftArrowButton;
   public GameObject _rightArrowButton;
   
   ExitGames.Client.Photon.Hashtable _playerProperties = new ExitGames.Client.Photon.Hashtable();
   public Image _playerAvatar;
   public Sprite[] _avatars;

   private Player _player;

   private void Start()
   {
      OnClickRightArrow();
   }

   public void SetPlayerInfo(Player player)
   {
      _playerName.text = player.NickName;
      _player = player;
      UpdatePlayerItem(_player);
   }

   public void ApplyLocalChanges()
   {
      _backgroundImage.color = _highlightColor;
      _leftArrowButton.SetActive(true);
      _rightArrowButton.SetActive(true);
   }

   public void OnClickLeftArrow()
   {
      if ((int)_playerProperties["playerAvatar"] == 0)
      {
         _playerProperties["playerAvatar"] = _avatars.Length - 1;
      }
      else
      {
         _playerProperties["playerAvatar"] = (int)_playerProperties["playerAvatar"] - 1;
      }
      PhotonNetwork.SetPlayerCustomProperties(_playerProperties);
   }

   public void OnClickRightArrow()
   {
      if ((int)_playerProperties["playerAvatar"] == _avatars.Length - 1)
      {
         _playerProperties["playerAvatar"] = 0;
      }
      else
      {
         _playerProperties["playerAvatar"] = (int)_playerProperties["playerAvatar"] + 1;
      }

      PhotonNetwork.SetPlayerCustomProperties(_playerProperties);
   }

   public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
   {
      if (_player == targetPlayer)
      {
         UpdatePlayerItem(targetPlayer);
      }
   }

   void UpdatePlayerItem(Player player)
   {
      if (player.CustomProperties.ContainsKey("playerAvatar"))
      {
         _playerAvatar.sprite = _avatars[(int)player.CustomProperties["playerAvatar"]];
         _playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
      }
      else
      {
         _playerProperties["playerAvatar"] = 0;
      }
   }
}
