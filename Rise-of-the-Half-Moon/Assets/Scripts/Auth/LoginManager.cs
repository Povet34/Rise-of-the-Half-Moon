using System;
using System.Collections;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Text UsernameTxt, UserEmailTxt;
    public Image UserProfilePic;
    public GameObject loginScreen, ProfileScreen;
    private FirebaseAuth firebaseAuthManager;

    private void Awake()
    {
        firebaseAuthManager = FindObjectOfType<FirebaseAuth>();
    }

    public void GoogleSignInClick()
    {
        firebaseAuthManager.GoogleSignInClick(OnGoogleSignInFinished);
    }

    public void GuestSignInClick()
    {
        firebaseAuthManager.SignInAnonymously(OnGuestSignInFinished);
    }

    private void OnGoogleSignInFinished(FirebaseUser user)
    {
        if (user != null)
        {
            UpdateUIWithUserInfo(user);
        }
    }

    private void OnGuestSignInFinished(FirebaseUser user)
    {
        if (user != null)
        {
            UpdateUIWithUserInfo(user);
        }
    }

    private void UpdateUIWithUserInfo(FirebaseUser user)
    {
        UsernameTxt.text = user.DisplayName;
        UserEmailTxt.text = user.Email;
        string imageUrl = user.PhotoUrl?.ToString();
        StartCoroutine(LoadProfilePic(imageUrl));
        loginScreen.SetActive(false);
        ProfileScreen.SetActive(true);

        firebaseAuthManager.CheckOrCreateUser(user.UserId, user.DisplayName, user.Email, imageUrl);
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
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(CheckImageURL(url));
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load profile picture: " + www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
    }
}