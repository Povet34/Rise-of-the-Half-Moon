using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button replayButton;
    [SerializeField] Button exitButton;

    GameManager gameManager;

    private void Awake()
    {
        replayButton.onClick.AddListener(Replay);
        exitButton.onClick.AddListener(Exit);
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        exitButton.gameObject.SetActive(gameManager is PVEGameManager pve);
    }

    public void ShowResult(string msg)
    {
        gameObject.SetActive(true);
        resultText.text = msg;
    }

    private void Replay()
    {
        if(gameManager is PVEGameManager pve)
        {
            ContentsDataManager.Instance.SetPVEGameInitData();
            SceneManager.LoadScene(Definitions.INGAME_SCENE);
        }
    }

    private void Exit()
    {
        if (gameManager.IsNetworkGame)
        {
            PhotonNetwork.LeaveRoom();
        }

        SceneManager.LoadSceneAsync(Definitions.LOBBY_SCENE);
    }
}
