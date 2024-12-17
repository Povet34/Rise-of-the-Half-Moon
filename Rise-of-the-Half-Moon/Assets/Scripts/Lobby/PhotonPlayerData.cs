using ExitGames.Client.Photon;
using Firebase.Database;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PhotonPlayerData : MonoBehaviourPunCallbacks
{
    string playerName;
    string playerEmail;
    string playerImageUrl;
    Sprite playerProfileSprite;
    int score;

    private void Start()
    {
        FirebaseAuth.Instance.GetAuthedUser(user =>
        {
            if (user != null)
            {
                playerName = user.DisplayName;
                playerEmail = user.Email;
                playerImageUrl = user.PhotoUrl != null ? user.PhotoUrl.ToString() : "";

                SetPlayerCustomProperties();
                StartCoroutine(DownloadAndSetProfileSprite(playerImageUrl));
                StartCoroutine(GetPlayerScoreFromFirebase(user.UserId));
            }
            else
            {
                Debug.LogError("No user is signed in.");
            }
        });
    }

    private void SetPlayerCustomProperties()
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Name", playerName },
            { "Email", playerEmail },
            { "ImageUrl", playerImageUrl },
            { "Score", score }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    private IEnumerator DownloadAndSetProfileSprite(string imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download profile picture: " + www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                playerProfileSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }
    }

    private IEnumerator GetPlayerScoreFromFirebase(string userId)
    {
        var dbReference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(userId).Child("score");
        var dbTask = dbReference.GetValueAsync();

        yield return new WaitUntil(() => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to retrieve score: {dbTask.Exception}");
        }
        else if (dbTask.Result.Value != null)
        {
            score = int.Parse(dbTask.Result.Value.ToString());
            SetPlayerCustomProperties(); // Update custom properties with the retrieved score
        }
        else
        {
            Debug.LogWarning("Score not found in Firebase.");
        }
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public string GetPlayerEmail()
    {
        return playerEmail;
    }

    public void SetPlayerEmail(string email)
    {
        playerEmail = email;
    }

    public string GetPlayerImageUrl()
    {
        return playerImageUrl;
    }

    public void SetPlayerImageUrl(string imageUrl)
    {
        playerImageUrl = imageUrl;
        StartCoroutine(DownloadAndSetProfileSprite(imageUrl));
    }

    public Sprite GetPlayerProfileSprite()
    {
        return playerProfileSprite;
    }

    public void SetPlayerProfileSprite(Sprite sprite)
    {
        playerProfileSprite = sprite;
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        SetPlayerCustomProperties(); // Update custom properties with the new score
    }
}