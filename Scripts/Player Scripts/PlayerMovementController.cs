using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementController : MonoBehaviour
{
    private PlayerMaster playerMaster;
    private BasicMovement basicMovemet;
    private MovingObjectMovement movingObjectMovement;
    private PlayerLifeSystem _playerLifeSystem;
    private LevelMaster _levelMaster;

    private Transform levelSpawn;

    public bool isOnMovingObject;

    private bool _cantMove;

    private void OnEnable()
    {
        SetInitialReferences();
        playerMaster.EventPlayerReset += ResetPlayer;
        playerMaster.EventPlayerDies += StopPlayerMovment;
        IngredientGotHandler._PlayerReturnToGame += ReturnPlayerToGame;
        IngredientGotHandler._PlayerCrafting += StopPlayerMovment;
    }

    private void OnDisable()
    {
        playerMaster.EventPlayerReset -= ResetPlayer;
        playerMaster.EventPlayerDies -= StopPlayerMovment;
        IngredientGotHandler._PlayerReturnToGame -= ReturnPlayerToGame;
        IngredientGotHandler._PlayerCrafting -= StopPlayerMovment;
    }

    void SetInitialReferences() 
    {
        _levelMaster = FindObjectOfType<LevelMaster>();
        levelSpawn = FindObjectOfType<LevelSpawnScript>().levelSpawn;
        basicMovemet = GetComponent<BasicMovement>();
        movingObjectMovement = GetComponent<MovingObjectMovement>();
        playerMaster = GetComponent<PlayerMaster>();
        _playerLifeSystem = GetComponent<PlayerLifeSystem>();
    }

    private void Update()
    {
        //Check if falling off of a moving object
        
        if (Vector3.Distance(transform.position, basicMovemet.destination) >= 2)
        {
            isOnMovingObject = false;
        }

        if (isOnMovingObject) 
        {
            movingObjectMovement.MoveWithObject();
        }

        if (!_cantMove)
        {
            basicMovemet.Move();
        }
        
        //Check what object you are standing on once you get to it
        if (!basicMovemet.isMoving) 
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity))
            {
                if (hitInfo.transform.gameObject.layer == 10) //Layer 10 = moving object layer
                {
                    isOnMovingObject = true;
                }
                else
                {
                    isOnMovingObject = false;
                }//put else if here for more options
            }
            else
            {
                //Not sure what to put here yet.
            }
        }
    }

    void ResetPlayer() 
    {
        //Temp Reset Player Code
        _cantMove = false;
        transform.position = levelSpawn.position;
        basicMovemet.destination = levelSpawn.position;
        basicMovemet.shouldFall = false;
        basicMovemet.isFalling = false;
        isOnMovingObject = false;
        basicMovemet.ResetScale();
    }

    void ReturnPlayerToGame()
    {
        _cantMove = false;
        basicMovemet.shouldFall = false;
        basicMovemet.isFalling = false;
        isOnMovingObject = false;
        basicMovemet.ResetScale();
    }

    public void StopPlayerMovment()
    {
        _cantMove = true;
    }

    public void UsePotion(int potionIndex)
    {
        if (potionIndex == 0)
        {
            _playerLifeSystem.ToggleExtraHit(true);
        }else if (potionIndex == 1)
        {
            Debug.Log("Used Score Potion");
            _levelMaster._scoreMulitplyer = 5;
        }else if (potionIndex == 2)
        {
            basicMovemet._speedBuffMultiplyer = 5;
        }
    }

}
