using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public void GoLobby(Action callback)
    {
        SceneManager.LoadSceneAsync("Lobby");
    }

    public void GoAuth(Action callback)
    {
        SceneManager.LoadSceneAsync("Auth");
    }

    public void GoGame(Action callback)
    {
        SceneManager.LoadSceneAsync("InGame");
    }
}
