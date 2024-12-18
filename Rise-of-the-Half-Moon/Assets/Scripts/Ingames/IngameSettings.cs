using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameSettings : MonoBehaviour
{
    [SerializeField] Button exitButton;
    [SerializeField] Button gobackButton;

    GameManager gameManager;

    private void Awake()
    {
        exitButton.onClick.AddListener(Exit);
        gobackButton.onClick.AddListener(Goback);
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Exit()
    {
        if(gameManager.IsNetworkGame)
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
