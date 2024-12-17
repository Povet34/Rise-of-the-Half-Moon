using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";
    private Lobby lobby;

    public event Action<PhotonPlayerData> OnPlayerEnteredRoomCallback;
    public event Action<PhotonPlayerData> OnPlayerAlreadyInRoomCallback;
    public event Action OnJoinedRoomCallback;

    private void Start()
    {
        lobby = GetComponent<Lobby>();
        if (lobby == null)
        {
            Debug.LogError("Lobby instance not found!");
            return;
        }

        ConnectToMaster();
    }

    private void ConnectToMaster()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master Server");

        if (lobby != null)
        {
            lobby.SetMatchmakingButtonInteractable(true);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected from Master Server: " + cause);

        if (lobby != null)
        {
            lobby.SetMatchmakingButtonInteractable(false);
        }
    }

    public void StartMatchmaking()
    {
        Debug.Log("Starting matchmaking...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room. Creating a new room...");
        CreateRoom();
    }

    private void CreateRoom()
    {
        int randomRoomNumber = UnityEngine.Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully.");
        OnJoinedRoomCallback?.Invoke();

        // 이미 방에 있는 유저의 정보를 가져와 콜백 호출
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            PhotonPlayerData opponentData = GetOpponentData(player);
            OnPlayerAlreadyInRoomCallback?.Invoke(opponentData);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player has entered the room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // 정원이 2명이 되면 상대 유저의 데이터를 가져옵니다.
            PhotonPlayerData opponentData = GetOpponentData(newPlayer);

            // 콜백 호출
            OnPlayerEnteredRoomCallback?.Invoke(opponentData);
        }
    }

    private PhotonPlayerData GetOpponentData(Player player)
    {
        PhotonPlayerData opponentData = new PhotonPlayerData();

        if (player.CustomProperties.TryGetValue("Name", out object name))
        {
            opponentData.SetPlayerName(name.ToString());
            Debug.Log("Opponent Name: " + name);
        }

        if (player.CustomProperties.TryGetValue("Email", out object email))
        {
            opponentData.SetPlayerEmail(email.ToString());
            Debug.Log("Opponent Email: " + email);
        }

        if (player.CustomProperties.TryGetValue("ImageUrl", out object imageUrl))
        {
            opponentData.SetPlayerImageUrl(imageUrl.ToString());
            Debug.Log("Opponent Image URL: " + imageUrl);
        }

        if (player.CustomProperties.TryGetValue("Score", out object score))
        {
            opponentData.SetScore(int.Parse(score.ToString()));
            Debug.Log("Opponent Score: " + score);
        }

        return opponentData;
    }

    public void CancelMatchmaking()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                // 방에 혼자 있는 경우 방을 파괴합니다.
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                // 방에 다른 플레이어가 있는 경우 방에서 나갑니다.
                PhotonNetwork.LeaveRoom();
            }
        }
    }
}