using Firebase.Auth;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public GameObject loginScreen;
    public AuthSceneProfile sceneProfile;

    public void GoogleSignInClick()
    {
        FirebaseAuth.Instance.GoogleSignInClick(OnGoogleSignInFinished);
    }

    public void GuestSignInClick()
    {
        FirebaseAuth.Instance.SignInAnonymously(OnGuestSignInFinished);
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
        loginScreen.SetActive(false);
        sceneProfile.gameObject.SetActive(true);

        FirebaseAuth.Instance.CheckOrCreateUser(user.UserId, user.DisplayName, user.Email, user.PhotoUrl?.ToString());

        if (sceneProfile != null)
        {
            sceneProfile.ViewUser();
        }
    }
}