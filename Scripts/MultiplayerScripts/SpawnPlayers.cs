using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{

   public GameObject[] _playerPrefabs;
   public Transform[] _spawnpoints;

   private Transform _spawnpoint;

   public static event Action _OnPlayerSpawn;
   private void Start()
   {
      GameObject playerToSpawn = _playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
      
      if (PhotonNetwork.IsMasterClient)
      {
         _spawnpoint = _spawnpoints[0];
      }
      else
      {
         _spawnpoint = _spawnpoints[1];
      }

      PhotonNetwork.Instantiate(playerToSpawn.name, _spawnpoint.position, Quaternion.identity);

      _OnPlayerSpawn?.Invoke();
   }
   
}
