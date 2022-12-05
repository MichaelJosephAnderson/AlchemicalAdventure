using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveFrogsManager : MonoBehaviour
{
    private PlayerMaster playerMaster;
    private LevelMaster levelMaster;
    
    [SerializeField] private int _numFrogsInLevel = 5;
    private int _currentNumFrogs;
    private void OnEnable()
    {
        SetInitialRefecrences();
        SpawnPlayers._OnPlayerSpawn += SetupPlayer;
        levelMaster.EventPlayerGetsBabyFrog += PlayerGetsFrog;
    }

    private void OnDisable()
    {
        SpawnPlayers._OnPlayerSpawn -= SetupPlayer;
        levelMaster.EventPlayerGetsBabyFrog -= PlayerGetsFrog;
    }

    private void SetupPlayer()
    {
        playerMaster = FindObjectOfType<PlayerMaster>();
    }

    private void SetInitialRefecrences()
    {
        _currentNumFrogs = 0;
        levelMaster = GetComponentInParent<LevelMaster>();
        //_numFrogsInLevel = FindObjectOfType<LevelObjectivesSpecifics>()._numObjectives;
    }
    
    private void PlayerGetsFrog()
    {
        levelMaster.CallEventScoreIncrease(100);

        _currentNumFrogs++;
        if (_currentNumFrogs != _numFrogsInLevel)
        {
            playerMaster.CallEventPlayerGetsIngredient();
        }
        else
        {
            levelMaster.CallEventLevelComplete();
            //playerMaster.CallEventPlayerGetsIngredient();
        }
    }

}
