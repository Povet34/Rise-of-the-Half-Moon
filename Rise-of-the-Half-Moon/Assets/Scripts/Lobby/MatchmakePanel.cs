using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakePanel : MonoBehaviour
{
    [SerializeField] Button cancelButton;
    [SerializeField] Image matchmakeLoadingImage;
    [SerializeField] float rotationSpeed = 100f;

    [SerializeField] MatchUserProfile myProfile;
    [SerializeField] MatchUserProfile otherProfile;

    private PhotonLobby photonLobby;
    private PhotonPlayerData playerData;

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

    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    private void ClosePanel()
    {
        if (photonLobby != null)
        {
            photonLobby.CancelMatchmaking();
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

    public void SetMyProfile(PhotonPlayerData myData)
    {
        myProfile.SetProfile(PhotonPlayerData.FromCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties));
    }

    public void SetOtherProfile(PhotonPlayerData opponentData)
    {
        if (opponentData != null)
        {
            otherProfile.SetProfile(opponentData);
            matchmakeLoadingImage.gameObject.SetActive(false);
            otherProfile.gameObject.SetActive(true);
        }
    }
}