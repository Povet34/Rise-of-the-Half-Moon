using ExitGames.Client.Photon;
using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class PhotonPlayerData
{
    public string PlayerName { get; private set; }
    public string PlayerEmail { get; private set; }
    public string PlayerImageUrl { get; private set; }
    public Sprite PlayerProfileSprite { get; private set; }
    public int Score { get; private set; }

    public async Task Initialize()
    {
        var user = GetAuthedUser();
        if (user != null)
        {
            PlayerName = user.DisplayName;
            PlayerEmail = user.Email;
            PlayerImageUrl = user.PhotoUrl != null ? user.PhotoUrl.ToString() : "";

            SetPlayerCustomProperties();
            await DownloadAndSetProfileSprite(PlayerImageUrl);
            await GetPlayerScoreFromFirebase(user.UserId);
        }
        else
        {
            Debug.LogError("No user is signed in.");
        }
    }

    private FirebaseUser GetAuthedUser()
    {
        return Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
    }

    private void SetPlayerCustomProperties()
    {
        Hashtable customProperties = new Hashtable
        {
            { "Name", PlayerName },
            { "Email", PlayerEmail },
            { "ImageUrl", PlayerImageUrl },
            { "Score", Score }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    private async Task DownloadAndSetProfileSprite(string imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download profile picture: " + www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                PlayerProfileSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }
    }

    private async Task GetPlayerScoreFromFirebase(string userId)
    {
        var dbReference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(userId).Child("score");
        var dbTask = dbReference.GetValueAsync();

        await Task.Run(() => dbTask.Wait());

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to retrieve score: {dbTask.Exception}");
        }
        else if (dbTask.Result.Value != null)
        {
            Score = int.Parse(dbTask.Result.Value.ToString());
            SetPlayerCustomProperties(); // Update custom properties with the retrieved score
        }
        else
        {
            Debug.LogWarning("Score not found in Firebase.");
        }
    }

    public static PhotonPlayerData FromCustomProperties(Hashtable customProperties)
    {
        PhotonPlayerData playerData = new PhotonPlayerData();
        if (customProperties.TryGetValue("Name", out object name))
        {
            playerData.PlayerName = name.ToString();
        }
        if (customProperties.TryGetValue("Email", out object email))
        {
            playerData.PlayerEmail = email.ToString();
        }
        if (customProperties.TryGetValue("ImageUrl", out object imageUrl))
        {
            playerData.PlayerImageUrl = imageUrl.ToString();
        }
        if (customProperties.TryGetValue("Score", out object score))
        {
            playerData.Score = int.Parse(score.ToString());
        }
        return playerData;
    }
}