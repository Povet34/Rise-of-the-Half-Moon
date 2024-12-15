using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakePanel : MonoBehaviour
{
    [SerializeField] Button cancelButton;
    [SerializeField] Image matchmakeLoadingImage;
    [SerializeField] float rotationSpeed = 100f;

    private PhotonLobby photonLobby;

    private void Awake()
    {
        cancelButton.onClick.AddListener(ClosePanel);
        photonLobby = FindObjectOfType<PhotonLobby>();
        if (photonLobby == null)
        {
            Debug.LogError("PhotonLobby instance not found!");
        }
    }

    private void Update()
    {
        RotateLoadingImage();
    }

    private void ClosePanel()
    {
        if (photonLobby != null)
        {
            photonLobby.CancelMatchmaking();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void RotateLoadingImage()
    {
        if (matchmakeLoadingImage != null)
        {
            matchmakeLoadingImage.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}