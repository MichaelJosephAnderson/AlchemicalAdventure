using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class LevelSpawnScript : MonoBehaviour
{
    [SerializeField] private Transform _p1Spawn;
    [SerializeField] private Transform _p2Spawn;
    [SerializeField] private List<Checkpoints> _checkpoints;

    public Transform levelSpawn;

    private Transform _currentRespawnPoint;
    private PhotonView _view;

    private void Start()
    {
        _view = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            levelSpawn.SetPositionAndRotation(_p1Spawn.position, Quaternion.identity);
        }
        else
        {
            levelSpawn.SetPositionAndRotation(_p2Spawn.position, Quaternion.identity);
        }
    }

    public void SetRespawnPoint(int index)
    {
        foreach (var point in _checkpoints)
        {
            if (point._checkpointIndex == index)
            {
                levelSpawn.SetPositionAndRotation(point.gameObject.transform.position, Quaternion.identity);
                //_currentRespawnPoint.position = point.gameObject.transform.position;
            }
        }
        //levelSpawn.SetPositionAndRotation(_currentRespawnPoint.position, Quaternion.identity);
    }

    public void ResetSpawnPoint()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            levelSpawn.SetPositionAndRotation(_p1Spawn.position, Quaternion.identity);
            Debug.Log("here");
        }
        else
        {
            levelSpawn.SetPositionAndRotation(_p2Spawn.position, Quaternion.identity);
        }
    }
}
