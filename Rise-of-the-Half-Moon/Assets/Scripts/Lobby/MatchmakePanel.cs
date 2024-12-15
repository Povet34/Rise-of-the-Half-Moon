using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakePanel : MonoBehaviour
{
    [SerializeField] Button cancelButton;
    [SerializeField] Image matchmakeLoadingImage;
    [SerializeField] float rotationSpeed = 100f;

    private void Awake()
    {
        cancelButton.onClick.AddListener(ClosePanel);
    }

    private void Update()
    {
        RotateLoadingImage();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    private void RotateLoadingImage()
    {
        if (matchmakeLoadingImage != null)
        {
            matchmakeLoadingImage.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}