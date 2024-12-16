using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class FirebaseAuth : Singleton<FirebaseAuth>
{
    private Firebase.Auth.FirebaseAuth auth;
    private FirebaseDatabase database;
    private DatabaseReference databaseReference;

    private string GoogleWebAPI = Definitions.GoogleWebAPI;
    private GoogleSignInConfiguration configuration;

    public FirebaseUser GetUser() => auth.CurrentUser;

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
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance;
        databaseReference = database.RootReference;
    }

    public void GoogleSignInClick(Action<FirebaseUser> callback)
    {
#if UNITY_ANDROID || UNITY_IOS
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.Configuration.RequestProfile = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task => OnAuthenticationFinished(task, callback));
#else
        Debug.LogWarning("Google Sign-In is only supported on Android and iOS platforms.");
        callback?.Invoke(null);
#endif
    }

    void OnAuthenticationFinished(Task<GoogleSignInUser> task, Action<FirebaseUser> callback)
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
                    callback?.Invoke(null);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    callback?.Invoke(null);
                    return;
                }

                FirebaseUser user = auth.CurrentUser;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                callback?.Invoke(user);
            });
        }
    }

    public void SignInAnonymously(Action<FirebaseUser> callback)
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                callback?.Invoke(null);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                callback?.Invoke(null);
                return;
            }

            FirebaseUser user = auth.CurrentUser;
            string guestName = GenerateGuestName();
            UpdateUserProfile(user, guestName, callback);
        });
    }

    private string GenerateGuestName()
    {
        return $"guest{DateTime.Now.Ticks}";
    }

    private void UpdateUserProfile(FirebaseUser user, string displayName, Action<FirebaseUser> callback)
    {
        UserProfile profile = new UserProfile { DisplayName = displayName };
        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                callback?.Invoke(null);
                return;
            }

            Debug.LogFormat("User profile updated successfully: {0}", user.DisplayName);
            callback?.Invoke(user);
        });
    }

    public void SignInWithEmailAndPassword(string email, string password, Action<FirebaseUser> callback)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                callback?.Invoke(null);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                callback?.Invoke(null);
                return;
            }

            FirebaseUser user = auth.CurrentUser;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            callback?.Invoke(user);
        });
    }

    public void SignOut()
    {
        auth.SignOut();
        Debug.Log("User signed out successfully.");
    }

    public void CheckOrCreateUser(string userId, string displayName, string email, string imgURL)
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

    public void GetAuthedUser(Action<FirebaseUser> callback)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            callback?.Invoke(user);
        }
        else
        {
            Debug.LogError("No user is signed in.");
            callback?.Invoke(null);
        }
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