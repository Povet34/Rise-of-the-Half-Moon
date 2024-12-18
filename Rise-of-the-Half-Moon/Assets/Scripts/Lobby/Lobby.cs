using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Lobby : MonoBehaviour
{
    [SerializeField] Button matchmakingButton;
    [SerializeField] Button playWithAIButton;
    [SerializeField] Button SettingsButton;

    [SerializeField] LobbySettings settings;
    [SerializeField] GlobalVolumeController globalVolumeController;
    [SerializeField] MatchmakePanel matchmakePanel;

    private PhotonLobby photonLobby;

    private void Awake()
    {
        photonLobby = GetComponent<PhotonLobby>();
        if (photonLobby == null)
        {
            Debug.LogError("PhotonLobby instance not found!");
            return;
        }

        matchmakingButton.onClick.AddListener(StartMatchMaring);
        playWithAIButton.onClick.AddListener(PlayWithAI);
        SettingsButton.onClick.AddListener(() => settings.gameObject.SetActive(true));

        photonLobby.OnPlayerEnteredRoomCallback += OnPlayerEnteredRoom;
        photonLobby.OnPlayerAlreadyInRoomCallback += OnPlayerAlreadyInRoom;
        photonLobby.OnJoinedRoomCallback += OnJoinedRoom;
        photonLobby.OnCreatedRoomCallback += OnCreatedRoom;
    }

    private void OnDestroy()
    {
        if (photonLobby != null)
        {
            photonLobby.OnPlayerEnteredRoomCallback -= OnPlayerEnteredRoom;
            photonLobby.OnPlayerAlreadyInRoomCallback -= OnPlayerAlreadyInRoom;
            photonLobby.OnJoinedRoomCallback -= OnJoinedRoom;
            photonLobby.OnCreatedRoomCallback -= OnCreatedRoom;
        }
    }

    public void SetMatchmakingButtonInteractable(bool interactable)
    {
        matchmakingButton.interactable = interactable;
    }

    private void PlayWithAI()
    {
        if (globalVolumeController != null)
        {
            globalVolumeController.SetLensDistortion(-1.0f, 0.01f, 1.0f);
            StartCoroutine(LoadGameSceneAfterDelay(2.0f));
        }
        else
        {
            Debug.LogError("GlobalVolumeController is not assigned.");
        }
    }

    private IEnumerator LoadGameSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartPVEGame();

        SceneManager.LoadScene(Definitions.INGAME_SCENE);
    }

    private void StartMatchMaring()
    {
        if (photonLobby != null)
        {
            photonLobby.StartMatchmaking();
        }
        else
        {
            Debug.LogError("PhotonLobby instance not found!");
        }
    }

    private void OnCreatedRoom(PhotonPlayerData myData)
    {
        if (matchmakePanel != null)
        {
            matchmakePanel.SetMyProfile(myData);
        }
    }

    private void OnPlayerAlreadyInRoom(PhotonPlayerData opponentData)
    {
        if (matchmakePanel != null)
        {
            matchmakePanel.SetOtherProfile(opponentData);
        }
    }

    private void OnPlayerEnteredRoom(PhotonPlayerData opponentData)
    {
        if (matchmakePanel != null)
        {
            matchmakePanel.SetOtherProfile(opponentData);
        }
    }


    private void OnJoinedRoom()
    {
        if (matchmakePanel != null)
        {
            matchmakePanel.ShowPanel();
        }
    }

    private void StartPVEGame()
    {
        PVEGameManager.GameInitData data = new PVEGameManager.GameInitData();
        data.contentType = (PhaseData.ContentType)Random.Range(0, (int)PhaseData.ContentType.Count);
        data.initBotLevel = Random.Range(0, ContentsDataManager.Instance.botLevelDatas.Count);
        data.seed = Random.Range(0, 100000);

        ContentsDataManager.Instance.SetPVEGameInitData(data);
        SceneManager.LoadScene(Definitions.INGAME_SCENE);
    }
}