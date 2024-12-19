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
    [SerializeField] PUNCardDrawer cardDrawerPrefab;

    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing on this GameObject.");
        }
    }

    public void StartGameInit()
    {
        IsNetworkGame = true;

        var go = PhotonNetwork.Instantiate("PUNCardDrawer", Vector3.zero, Quaternion.identity);
        cardDrawer = go.GetComponent<PUNCardDrawer>();

        GameInitData initData = ContentsDataManager.Instance.GetPVPGameInitData();
        photonView.RPC(nameof(GameInit), RpcTarget.All, (int)initData.contentType, initData.seed, go.GetPhotonView().ViewID);
    }

    [PunRPC]
    public void GameInit(int type, int seed, int viewID)
    {
        Random.InitState(seed);

        contentType = (PhaseData.ContentType)type;
        phaseDatas = ContentsDataManager.Instance.GetPhaseDatas(contentType, ref rule);

        StartPlay(viewID);
    }

    private void StartPlay(int viewID)
    {
        foreach (var card in otherCards)
        {
            card.Destroy();
        }

        foreach (var card in myCards)
        {
            card.Destroy();
        }

        myCards.Clear();
        otherCards.Clear();

        nodeGenerator.Create();
        rule.Init();

        if(null == cardDrawer)
            cardDrawer = PhotonView.Find(viewID).GetComponent<PUNCardDrawer>();

        cardDrawer.Init(phaseDatas, ref myCards, ref otherCards, NextTurn);

        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        isMyTurn = PhotonNetwork.IsMasterClient; // 마스터 클라이언트가 먼저 시작
        cardDrawer.DrawCard(isMyTurn);
    }

    private void InitCards(int cardCount, List<ICard> cards, bool isPlayer1)
    {
        for (int i = 0; i < cardCount; i++)
        {
            cardDrawer.DrawCard(isPlayer1, false);
        }
    }

    private void NextTurn(ICard removedCard)
    {
        if (nodeGenerator.IsEndGame())
        {
            SettlementPlay();
            return;
        }

        //리스트에서 제거하고
        if (isMyTurn)
            myCards.Remove(removedCard);
        else
            otherCards.Remove(removedCard);

        //턴을 바꿔주고
        isMyTurn = !isMyTurn;

        //드로우한다
        cardDrawer.DrawCard(isMyTurn);
    }


    private void SettlementPlay()
    {
        Rule.SettlementOccupiedNodes(
            () =>
            {
                if (myScore > otherScore)
                {
                    gameUI.ShowWin();
                }
                else if (myScore < otherScore)
                {
                    gameUI.ShowLose();
                }
                else
                {
                    gameUI.ShowDraw();
                }
            });
    }

    #region Update Score

    public override void UpdateMyScore(int score)
    {
        myScore += score;
        gameUI.UpdateMyScore(myScore);
    }

    public override void UpdateOtherScore(int score)
    {
        otherScore += score;
        gameUI.UpdateOtherScore(otherScore);
    }

    #endregion
}