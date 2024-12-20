using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PVPGameManager : GameManager
{
    [Serializable]
    public class GameInitData
    {
        public PhaseData.ContentType contentType;
        public PhotonPlayerData myPlayerData;
        public PhotonPlayerData otherPlayerData;
        public int seed;
    }

    PhotonView photonView;
    GameInitData data;

    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing on this GameObject.");
        }
    }

    protected override void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        IsNetworkGame = true;

        var go = PhotonNetwork.Instantiate(Definitions.PUNCardDrawer, Vector3.zero, Quaternion.identity);
        cardDrawer = go.GetComponent<PUNCardDrawer>();

        data = ContentsDataManager.Instance.GetPVPGameInitData();
        photonView.RPC(nameof(GameInit), RpcTarget.All, (int)data.contentType, data.seed, go.GetPhotonView().ViewID);
    }

    [PunRPC]
    public void GameInit(int type, int seed, int viewID)
    {
        Random.InitState(seed);
        StartPlay(type, viewID);
    }

    private void StartPlay(int type, int viewID)
    {
        InitNodeGenerator();
        InitCardList();
        InitRule(type);
        InitCam();

        if (null == cardDrawer)
            cardDrawer = PhotonView.Find(viewID).GetComponent<PUNCardDrawer>();

        InitCardDrawer();

        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        isMyTurn = PhotonNetwork.IsMasterClient; // 마스터 클라이언트가 먼저 시작
        cardDrawer.DrawCard(isMyTurn);

        ContentsDataManager.Instance.ClearDatas();
    }
}