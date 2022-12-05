using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LevelMaster : MonoBehaviour
{
    private PlayerMaster playerMaster;
    private LevelSpawnScript _levelSpawnScript;
    
    public int _levelIndex;
    
    [SerializeField] private TMP_Text scoreText;
    public int _score = 0;
    public int _scoreMulitplyer = 1;
    
    [SerializeField] private Slider timer;
    [SerializeField] private float levelTime;
    public List<Image> _hearts;
    [SerializeField] private Canvas _gameOverCanvas;
    [SerializeField] private GameObject _spectateView;
    [SerializeField] private GameObject _respawnStuff;
    [SerializeField] private Image _highlightYes;
    [SerializeField] private Image _highlightNo;
    [SerializeField] private TMP_Text _scoreText;

    public delegate void GeneralEventHandler();
    public event GeneralEventHandler EventPlayerGetsBabyFrog;
    public event GeneralEventHandler EventLevelComplete;
    public event GeneralEventHandler EventTimerHitsZero;

    public delegate void ScoreEventHandler(int scoreToAdd);
    public event ScoreEventHandler EventScoreIncrease;

    public event Action OnPlayerRespawn;
    
    public float _levelTimer;

    private bool _pauseTimer;
    private bool _gameOver;
    private bool _retry = true;

    private Scene _currentScene;
    public void CallEventPlayerGetsBabyFrog()
    {
        if (EventPlayerGetsBabyFrog != null)
        {
            EventPlayerGetsBabyFrog();
        }
    }
    
    public void CallEventLevelComplete()
    {
        if (EventLevelComplete != null)
        {
            EventLevelComplete();
        }
    }

    public void CallEventTimerHitsZero()
    {
        if (EventTimerHitsZero != null)
        {
            EventTimerHitsZero();
        }
    }

    public void CallEventScoreIncrease(int scoreToAdd)
    {
        if (EventScoreIncrease != null)
        {
            EventScoreIncrease(scoreToAdd);
        }
    }

    public void CallOnPlayerRespawn()
    {
        OnPlayerRespawn?.Invoke();
    }

    private void OnEnable()
    {
        SetInitialReferences();
        SpawnPlayers._OnPlayerSpawn += SetupPlayer;
        EventPlayerGetsBabyFrog += PauseTimer;
        EventScoreIncrease += AddToScore;
    }
    private void OnDisable()
    {
        SpawnPlayers._OnPlayerSpawn -= SetupPlayer;
        EventPlayerGetsBabyFrog -= PauseTimer;
        EventScoreIncrease -= AddToScore;
        playerMaster.EventPlayerGetsHit -= ResetTimer;
        playerMaster.EventPlayerDies -= GameOver;
    }

    private void SetupPlayer()
    {
        playerMaster = FindObjectOfType<PlayerMaster>();
        playerMaster.EventPlayerGetsHit += ResetTimer;
        playerMaster.EventPlayerDies += GameOver;
    }

    private void SetInitialReferences()
    {
        _levelTimer = levelTime;
        _currentScene = SceneManager.GetActiveScene();
        _levelSpawnScript = GetComponentInChildren<LevelSpawnScript>();
    }

    private void Update()
    {
        if (_gameOver)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _highlightYes.gameObject.SetActive(true);
                _highlightNo.gameObject.SetActive(false);
                _retry = true;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _highlightYes.gameObject.SetActive(false);
                _highlightNo.gameObject.SetActive(true);
                _retry = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_retry)
                {
                    _levelSpawnScript.ResetSpawnPoint();

                    CallOnPlayerRespawn();
                    _gameOverCanvas.gameObject.SetActive(false);
                    playerMaster.CallEventPlayerResets();
                }
                else
                {
                   _respawnStuff.SetActive(false);
                }
                _gameOver = false;
            }
            
        }

        //handleTimer
        if (_levelTimer <= 0)
        {
           CallEventTimerHitsZero();
           ResetTimer();
        }
        else
        {
            timer.maxValue = levelTime;
            timer.value = _levelTimer;
            if (!_pauseTimer)
            {
                //_levelTimer -= Time.deltaTime;
            }
        }
        //handle score
        scoreText.text = _score.ToString();
    }

    public void PauseTimer()
    {
        _pauseTimer = true;
    }

    private void UnpauseTimer()
    {
        _pauseTimer = false;
        ResetTimer();
    }

    private void ResetTimer()
    {
        _levelTimer = levelTime;
    }

    private void AddToScore(int add)
    {
        _score += add * _scoreMulitplyer;
    }

    void GameOver()
    {
        _scoreText.text = "Level Score: " + _score.ToString();
        _gameOverCanvas.gameObject.SetActive(true);
        PauseTimer();
        _gameOver = true;
    }
}
