using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySettings : MonoBehaviour
{
    [SerializeField] Button logoutButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button gobackButton;

    private void Awake()
    {
        logoutButton.onClick.AddListener(SignOut);
        exitButton.onClick.AddListener(ApplicationQuit);
        gobackButton.onClick.AddListener(Goback);
    }

    private void ApplicationQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SignOut()
    {
        FirebaseAuth.Instance.SignOut();
        SceneManager.LoadSceneAsync(Definitions.AUTH_SCENE);
    }

    private void Goback()
    {
        gameObject.SetActive(false);
    }
}
