using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";
    private Lobby lobby;

    public event Action<PhotonPlayerData> OnPlayerEnteredRoomCallback;
    public event Action<PhotonPlayerData> OnPlayerAlreadyInRoomCallback;
    public event Action<PhotonPlayerData> OnCreatedRoomCallback;
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
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void ConnectToMaster()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override async void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master Server");

        if (lobby != null)
        {
            lobby.SetMatchmakingButtonInteractable(true);
            await new PhotonPlayerData().Initialize();
        }
    }

    public void DisconnectFromMaster()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Debug.Log("Disconnected from Master Server.");
        }
        else
        {
            Debug.LogWarning("Not connected to any server.");
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

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Room created successfully.");

        PhotonPlayerData myData = PhotonPlayerData.FromCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        OnCreatedRoomCallback?.Invoke(myData);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully.");
        OnJoinedRoomCallback?.Invoke();

        // 이미 방에 있는 유저의 정보를 가져와 콜백 호출
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            PhotonPlayerData opponentData = PhotonPlayerData.FromCustomProperties(player.CustomProperties);
            OnPlayerAlreadyInRoomCallback?.Invoke(opponentData);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player has entered the room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // 정원이 2명이 되면 상대 유저의 데이터를 가져옵니다.
            PhotonPlayerData opponentData = PhotonPlayerData.FromCustomProperties(newPlayer.CustomProperties);

            // 콜백 호출
            OnPlayerEnteredRoomCallback?.Invoke(opponentData);

            StartCoroutine(DelayStart());
        }
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(3f);
        StartPVPGame();
    }

    public void CancelMatchmaking()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    public void StartPVPGame()
    {
        PhotonPlayerData myData = PhotonPlayerData.FromCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        PVPGameManager.GameInitData initData = new PVPGameManager.GameInitData
        {
            contentType = (PhaseData.ContentType)Random.Range(0, (int)PhaseData.ContentType.Count),
            myPlayerData = myData,
            otherPlayerData = PhotonPlayerData.FromCustomProperties(PhotonNetwork.PlayerListOthers[0].CustomProperties),
            seed = Random.Range(0, 10000),
        };

        ContentsDataManager.Instance.SetPVPGameInitData(initData);
        PhotonNetwork.LoadLevel(Definitions.INGAME_SCENE);
    }
}