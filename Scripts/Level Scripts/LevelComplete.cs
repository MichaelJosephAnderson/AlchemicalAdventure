using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    private LevelMaster levelMaster;
    private PlayerMovementController _playerMovementController;

    [SerializeField] private Canvas _LevelCompleteCanvase;
    [SerializeField] private Image _highlightYes;
    [SerializeField] private Image _highlightNo;
    [SerializeField] private TMP_Text _scoreText;

    private Scene _currentScene;
    private bool _levelComplete;
    private bool _continue = true;
    private void OnEnable()
    {
        SetInitialReferences();
        SpawnPlayers._OnPlayerSpawn += SetupPlayer;
        levelMaster.EventLevelComplete += LoadTitleScreen;
    }

    private void OnDisable()
    {
        SpawnPlayers._OnPlayerSpawn -= SetupPlayer;
        levelMaster.EventLevelComplete -= LoadTitleScreen;
    }

    private void SetupPlayer()
    {
        _playerMovementController = FindObjectOfType<PlayerMovementController>();
    }

    private void SetInitialReferences()
    {
        levelMaster = GetComponentInParent<LevelMaster>();
        _currentScene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        if (_levelComplete)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PhotonNetwork.LoadLevel("LevelSelect");
                }
                /*
                
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    _highlightYes.gameObject.SetActive(true);
                    _highlightNo.gameObject.SetActive(false);
                    _continue = true;
                }

                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _highlightYes.gameObject.SetActive(false);
                    _highlightNo.gameObject.SetActive(true);
                    _continue = false;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_continue)
                    {
                       PhotonNetwork.LoadLevel("LevelSelect");
                    }
                    else
                    {
                        PhotonNetwork.Disconnect();
                        SceneManager.LoadScene("TitleScreen");
                    }
                }    
                */
            }
        }
    }

    public void LoadTitleScreen()
    {
        _scoreText.text = "Level Score: " + levelMaster._score.ToString();
        _playerMovementController.StopPlayerMovment();
        _LevelCompleteCanvase.gameObject.SetActive(true);
        _levelComplete = true;
        levelMaster.PauseTimer();
        //SceneManager.LoadScene("TempTitle");
    }
}
