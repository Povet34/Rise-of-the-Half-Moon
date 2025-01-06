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
    [SerializeField] Button TestPVEButton;
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

        TestPVEButton.gameObject.SetActive(Application.isEditor);

        matchmakingButton.onClick.AddListener(StartMatchMaring);
        playWithAIButton.onClick.AddListener(PlayWithAI);
        SettingsButton.onClick.AddListener(() => settings.gameObject.SetActive(true));
        TestPVEButton.onClick.AddListener(StartTestPVEGame);

        photonLobby.OnPlayerEnteredRoomCallback += OnPlayerEnteredRoom;
        photonLobby.OnPlayerAlreadyInRoomCallback += OnPlayerAlreadyInRoom;
        photonLobby.OnJoinedRoomCallback += OnJoinedRoom;
        photonLobby.OnCreatedRoomCallback += OnCreatedRoom;

        SoundManager.Instance.PlayBGM(Definitions.SOUND_BGM1);
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
        if(matchmakingButton)
            matchmakingButton.interactable = interactable;
    }

    private void PlayWithAI()
    {
        if (globalVolumeController != null)
        {
            photonLobby.DisconnectFromMaster();
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
        ContentsDataManager.Instance.SetPVEGameInitData();
        SceneManager.LoadScene(Definitions.INGAME_SCENE);
    }

    private void StartTestPVEGame()
    {
        ContentsDataManager.Instance.SetTestData();
        SceneManager.LoadScene(Definitions.INGAME_SCENE);
    }
}