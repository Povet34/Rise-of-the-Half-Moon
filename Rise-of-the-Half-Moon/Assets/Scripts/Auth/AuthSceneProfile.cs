using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase.Auth;

public class AuthSceneProfile : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] Text userName;
    [SerializeField] Text userEmail;
    [SerializeField] Button goLobbyButton;

    [SerializeField] private Sprite defaultProfileSprite;

    private void Awake()
    {
        goLobbyButton.onClick.AddListener(GotoLobby);
    }

    public void ViewUser()
    {
        FirebaseUser user = FirebaseAuth.Instance.GetUser();
        if (user != null)
        {
            UpdateUIWithUserInfo(user);
        }
    }

    private void GotoLobby()
    {
        SceneManager.LoadSceneAsync(Definitions.LOBBY_SCENE);
    }

    private void UpdateUIWithUserInfo(FirebaseUser user)
    {
        userName.text = user.DisplayName;
        userEmail.text = user.Email;
        string imageUrl = user.PhotoUrl?.ToString();
        StartCoroutine(LoadProfilePic(imageUrl));
    }

    private string CheckImageURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }

        return null;
    }

    private IEnumerator LoadProfilePic(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            profileImage.sprite = defaultProfileSprite;
        }
        else
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(CheckImageURL(url));
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load profile picture: " + www.error);
                profileImage.sprite = defaultProfileSprite;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }
    }
}