using System;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public void GoLobby(Action callback)
    {
        SceneManager.LoadSceneAsync(Definitions.LOBBY_SCENE);
    }

    public void GoAuth(Action callback)
    {
        SceneManager.LoadSceneAsync(Definitions.AUTH_SCENE);
    }

    public void GoGame(Action callback)
    {
        SceneManager.LoadSceneAsync(Definitions.INGAME_SCENE);
    }
}
