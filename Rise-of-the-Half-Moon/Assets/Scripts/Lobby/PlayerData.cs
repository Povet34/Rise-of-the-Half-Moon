using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerData : MonoBehaviourPunCallbacks
{
    public string playerName;
    public string playerEmail;
    public string playerImageUrl;

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
            }
            else
            {
                Debug.LogError("No user is signed in.");
            }
        });
    }

    private void SetPlayerCustomProperties()
    {
        Hashtable customProperties = new Hashtable
        {
            { "Name", playerName },
            { "Email", playerEmail },
            { "ImageUrl", playerImageUrl }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }


    public string GetPlayerName()
    {
        return playerName;
    }

    public string GetPlayerEmail()
    {
        return playerEmail;
    }

    public string GetPlayerImageUrl()
    {
        return playerImageUrl;
    }
}