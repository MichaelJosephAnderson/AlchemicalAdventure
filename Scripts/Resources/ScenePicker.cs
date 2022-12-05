using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ScenePicker : MonoBehaviour
{
    [SerializeField]
    public string scenePath;

    public void LoadScene()
    {
        //SceneManager.LoadScene(scenePath);
        PhotonNetwork.LoadLevel(scenePath);
    }
    public void LoadSceneAdditive()
    {
        SceneManager.LoadScene(scenePath,LoadSceneMode.Additive);
    }
    public void LoadSceneAsync()
    {
        SceneManager.LoadSceneAsync(scenePath);
    }

    public void ReloadScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void UnloadThisScene()
    {
        SceneManager.UnloadSceneAsync(scenePath);
    }
}