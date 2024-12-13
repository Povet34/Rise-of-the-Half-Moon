using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] Button matchmarkingButton;
    [SerializeField] Button playWithAIButton;
    [SerializeField] Button SettingsButton;

    [SerializeField] Settings settings;
    [SerializeField] GlobalVolumeController globalVolumeController;

    private void Awake()
    {
        matchmarkingButton.onClick.AddListener(StartMatchMaring);
        playWithAIButton.onClick.AddListener(PlayWithAI);
        SettingsButton.onClick.AddListener(() => settings.gameObject.SetActive(true));
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
        SceneManager.LoadScene(Definitions.INGAME_SCENE);
    }

    private void StartMatchMaring()
    {

    }
}