using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
   [SerializeField] public int _checkpointIndex;
   [SerializeField] private ParticleSystem _particleSystem;
   private LevelSpawnScript _levelSpawnScript;

   private void Start()
   {
      _levelSpawnScript = FindObjectOfType<LevelSpawnScript>();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         if (other.GetComponentInParent<PhotonView>().IsMine)
         {
            if (!_particleSystem.isEmitting)
            {
               _particleSystem.Play();  
            }
            _levelSpawnScript.SetRespawnPoint(_checkpointIndex);
         }
      }
   }
}
