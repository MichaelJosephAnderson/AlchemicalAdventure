using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLifeSystem : MonoBehaviour
{
    private PlayerMaster playerMaster;
    private LevelMaster levelMaster;

    private Scene _currentScene;
    
    [SerializeField] private ParticleSystem _extraHitPS;

    public int Lives;
    public bool _extraHit;

    private int baseLives = 3;

    private void OnEnable()
    {
        SetInitialReferences();
        playerMaster.EventPlayerGetsHit += LooseALife;
        playerMaster.EventPlayerGainsALife += GainALife;
        playerMaster.EventPlayerDies += GameOver;
        levelMaster.EventTimerHitsZero += LooseALife;
        levelMaster.OnPlayerRespawn += Respawn;
        Lives = baseLives;
    }

    private void OnDisable()
    {
        playerMaster.EventPlayerDies -= GameOver;
        playerMaster.EventPlayerGainsALife -= GainALife;
        playerMaster.EventPlayerGetsHit -= LooseALife;
        levelMaster.EventTimerHitsZero -= LooseALife;
        levelMaster.OnPlayerRespawn -= Respawn;
    }

    void LooseALife() 
    {
        if (!_extraHit)
        {
            Lives--;
            levelMaster._hearts[Lives].gameObject.SetActive(false);
            KillPlayer();

            if (Lives == 0) 
            {
                playerMaster.CallEventPlayerDies();
            }
        }
        else
        {
            ToggleExtraHit(false);
        }
    }

    public void GainALife() 
    {
        Lives++;
        levelMaster._hearts[Lives].gameObject.SetActive(true);
    }

    public void Respawn()
    {
        Lives = baseLives;
        foreach (var heart in levelMaster._hearts)
        {
            heart.gameObject.SetActive(true);
        }
    }


    void SetInitialReferences() 
    {
        playerMaster = GetComponent<PlayerMaster>();
        _currentScene = SceneManager.GetActiveScene();
        levelMaster = FindObjectOfType<LevelMaster>();
    }

    public void ToggleExtraHit(bool on)
    {
        if (on)
        {
            _extraHitPS.gameObject.SetActive(true);
            _extraHit = true;
        }
        else
        {
            _extraHitPS.gameObject.SetActive(false);
            _extraHit = false;
        }
    }

    void KillPlayer() 
    {
        playerMaster.CallEventPlayerResets();
    }

    void GameOver() 
    {
        /*
        Debug.Log("TempGameOver");
        SceneManager.LoadScene(_currentScene.name);
        */
    }
}
