using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] Button matchmarkingButton;
    [SerializeField] Button playWithAIButton;
    [SerializeField] Button SettingsButton;

    [SerializeField] Settings settings;

    private void Awake()
    {
        matchmarkingButton.onClick.AddListener(null);
        playWithAIButton.onClick.AddListener(null);
        SettingsButton.onClick.AddListener(() => settings.gameObject.SetActive(true));
    }
}
