using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseDatabase database;
    private DatabaseReference databaseReference;

    public string GoogleWebAPI = "280826413127-uotcphrkd8rictnid8hadd0p0gn87ihl.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    public Text UsernameTxt, UserEmailTxt;
    public Image UserProfilePic;
    public string imageUrl;
    public GameObject loginScreen, ProfileScreen;

    [SerializeField] private Texture2D defaultProfileTexture;

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
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance;
        databaseReference = database.RootReference;
    }

    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.Configuration.RequestProfile = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
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

                FirebaseUser user = auth.CurrentUser;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                UsernameTxt.text = user.DisplayName;
                UserEmailTxt.text = user.Email;
                imageUrl = user.PhotoUrl.ToString();
                StartCoroutine(LoadProfilePic(imageUrl));
                loginScreen.SetActive(false);
                ProfileScreen.SetActive(true);

                CheckOrCreateUser(user.UserId, user.DisplayName, user.Email, imageUrl);
            });
        }
    }

    private void CheckOrCreateUser(string userId, string displayName, string email, string imgURL)
    {
        databaseReference.Child("users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Failed to retrieve user data: {task.Exception?.Message}");
                return;
            }

            if (task.Result.Exists)
            {
                Debug.Log($"User already exists: {userId}");
                // Retrieve existing user data
                string existingName = task.Result.Child("displayName").Value?.ToString();
                string existingEmail = task.Result.Child("email").Value?.ToString();
                string existingImgURL = task.Result.Child("imgURL").Value?.ToString();
                int existingScore = int.Parse(task.Result.Child("score").Value?.ToString() ?? "0");
                Debug.Log($"Name: {existingName}, Email: {existingEmail}, Image URL: {existingImgURL}, Score: {existingScore}");
            }
            else
            {
                Debug.Log("User does not exist. Creating a new user...");
                CreateUser(userId, displayName, email, imgURL);
            }
        });
    }

    private void CreateUser(string userId, string displayName, string email, string imgURL)
    {
        User newUser = new User(displayName, email)
        {
            imgURL = imgURL,
            score = 0 // 초기 점수 설정
        };
        string json = JsonUtility.ToJson(newUser);

        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Failed to create user: {task.Exception?.Message}");
                return;
            }

            Debug.Log($"User created successfully: {userId}");
        });
    }

    private string CheckImageURL(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            return url;
        }

        return imageUrl;
    }

    IEnumerator LoadProfilePic(string url)
    {
        Texture2D texture = null;

        if (string.IsNullOrEmpty(url))
        {
            texture = defaultProfileTexture;
        }
        else
        {
            WWW www = new WWW(CheckImageURL(url));
            yield return www;
            texture = www.texture;
        }

        UserProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, 400, 400), new Vector2(0, 0));
    }
}

[System.Serializable]
public class User
{
    public string displayName;
    public string email;
    public int score;
    public string imgURL;

    public User(string displayName, string email)
    {
        this.displayName = displayName;
        this.email = email;
    }
}