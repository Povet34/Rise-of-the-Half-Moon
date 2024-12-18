using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameSettings : MonoBehaviour
{
    [SerializeField] Button exitButton;
    [SerializeField] Button gobackButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(Exit);
        gobackButton.onClick.AddListener(Goback);
    }

    private void Exit()
    {
        if(GameManager.Instance.IsNetworkGame)
        {
            PhotonNetwork.LeaveRoom();
        }

        SceneManager.LoadSceneAsync(Definitions.LOBBY_SCENE);
    }

    private void Goback()
    {
        gameObject.SetActive(false);
    }
}
