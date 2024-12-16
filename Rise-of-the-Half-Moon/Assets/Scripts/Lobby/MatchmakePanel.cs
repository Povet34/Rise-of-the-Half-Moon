using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MatchmakePanel : MonoBehaviour
{
    [SerializeField] Button cancelButton;
    [SerializeField] Image matchmakeLoadingImage;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] Text myProfileName;
    [SerializeField] Image myProfileImage;

    private PhotonLobby photonLobby;
    private PlayerData playerData;

    private void Awake()
    {
        cancelButton.onClick.AddListener(ClosePanel);
        photonLobby = FindObjectOfType<PhotonLobby>();
        playerData = FindObjectOfType<PlayerData>();

        if (photonLobby == null)
        {
            Debug.LogError("PhotonLobby instance not found!");
        }

        if (playerData == null)
        {
            Debug.LogError("PlayerData instance not found!");
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

    public void SetMyProfile()
    {
        if (playerData != null)
        {
            myProfileName.text = playerData.GetPlayerName();

            string imageUrl = playerData.GetPlayerImageUrl();
            if (!string.IsNullOrEmpty(imageUrl))
            {
                StartCoroutine(LoadImage(imageUrl));
            }
        }
    }

    private IEnumerator LoadImage(string imageUrl)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load profile image: " + uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                myProfileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }
}