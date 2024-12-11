using Firebase.Extensions;
using Google;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignInManager : MonoBehaviour
{
    public string GoogleWebAPI = "280826413127-uotcphrkd8rictnid8hadd0p0gn87ihl.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public Text UsernameTxt, UserEmailTxt;
    public Image UserProfilePic;
    public string imageUrl;
    public GameObject loginScreen, ProfileScreen;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true
        };
    }

    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.Configuration.RequestProfile = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
            OnAuthenticationFinished);
    }

    void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception");
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => 
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                user = auth.CurrentUser;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                UsernameTxt.text = user.DisplayName;
                UserEmailTxt.text = user.Email;
                imageUrl = user.PhotoUrl.ToString();
                StartCoroutine(LoadProfilePic(imageUrl));
                loginScreen.SetActive(false);
                ProfileScreen.SetActive(true);
            });
        }
    }

    private string CheckImageURL(string url)
    {
        //if (url.Contains("s96-c"))
        //{
        //    url = url.Replace("s96-c", "s400-c");
        //}
        //return url;

        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }

        return imageUrl;
    }

    IEnumerator LoadProfilePic(string url)
    {
        WWW www = new WWW(CheckImageURL(url));
        yield return www;
        UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, 400, 400), new Vector2(0, 0));
    }
}
