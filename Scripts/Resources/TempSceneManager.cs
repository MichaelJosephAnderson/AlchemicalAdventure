using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneManager : MonoBehaviour
{
    public void LoadLevelOne()
    {
        SceneManager.LoadScene("Retro1-1");
    }

    public void LoadLevelTwo()
    {
        SceneManager.LoadScene("LillyPad-2BowWowFalls");
    }

    public void LoadTestScene()
    {
        SceneManager.LoadScene("TestScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
