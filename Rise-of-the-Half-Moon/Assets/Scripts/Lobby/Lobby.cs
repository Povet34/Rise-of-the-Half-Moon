using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] Button matchmakingButton;
    [SerializeField] Button playWithAIButton;
    [SerializeField] Button SettingsButton;

    [SerializeField] Settings settings;
    [SerializeField] GlobalVolumeController globalVolumeController;

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

        SetGameData();

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

    private void SetGameData()
    {
        var data = new PVEGameManager.GameInitData();
        data.contentType = (PhaseData.ContentType)Random.Range(0, (int)PhaseData.ContentType.Count);
        data.initBotLevel = Random.Range(0, ContentsDataManager.Instance.botLevelDatas.Count);

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == Definitions.INGAME_SCENE)
            {
                var gameManager = FindObjectOfType<PVEGameManager>();
                gameManager.GameInit(data);
            }
        };
    }
}