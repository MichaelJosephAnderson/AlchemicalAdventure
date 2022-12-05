using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private Scene _currentScene;
    
    private int _storyIndex;
    private bool _updateUI;

    private void Start()
    {
        _storyIndex = 0;
        _updateUI = false;
        _currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentScene.name == "WinScreen")
            {
                SceneManager.LoadScene("TitleScreen");
            }
            else
            {
                SceneManager.LoadScene("LoadingScene");
            }
        }
    }
}
