using System;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public void GoLobby()
    {
        SceneManager.LoadSceneAsync(Definitions.LOBBY_SCENE);
    }

    public void GoAuth()
    {
        SceneManager.LoadSceneAsync(Definitions.AUTH_SCENE);
    }

    public void GoGame()
    {
        SceneManager.LoadSceneAsync(Definitions.INGAME_SCENE);
    }
}
