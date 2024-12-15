using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] MatchmakePanel matchmakePanel;

    private readonly string gameVersion = "1";
    private Lobby lobby;

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
        matchmakePanel.gameObject.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room. Creating a new room...");
        CreateRoom();
    }

    private void CreateRoom()
    {
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully.");
        // 로딩 화면을 보여줍니다.
        matchmakePanel.gameObject.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player has entered the room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // 정원이 2명이 되면 게임 씬으로 이동합니다.
            SceneManager.LoadScene(Definitions.INGAME_SCENE);
        }
    }
}