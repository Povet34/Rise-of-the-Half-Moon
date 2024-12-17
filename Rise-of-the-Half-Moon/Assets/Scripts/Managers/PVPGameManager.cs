using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPGameManager : MonoBehaviourPunCallbacks
{
    public class GameInitData
    {
        public PhaseData.ContentType contentType;
        public PhotonPlayerData myPlayerData;
        public PhotonPlayerData otherPlayerData;
    }

    [Header("Data")]
    public bool isMyTurn;
    public PhaseData.ContentType contentType;

    ContentRule rule;
    public ContentRule Rule => rule;

    List<PhaseData> phaseDatas;

    System.Random random;
    NodeGenerator nodeGenerator;

    [Header("Me")]
    int myScore;
    List<Card> myCards = new List<Card>();

    [Header("Other")]
    int otherScore;
    List<Card> otherCards = new List<Card>();

    GameUI gameUI;
    CardDrawer cardDrawer;

    private void Awake()
    {
        nodeGenerator = FindAnyObjectByType<NodeGenerator>();
        gameUI = FindAnyObjectByType<GameUI>();
        cardDrawer = FindAnyObjectByType<CardDrawer>();
    }

    public void GameInit(GameInitData initData)
    {
        if (null == initData)
            return;

        contentType = initData.contentType;

        phaseDatas = ContentsDataManager.Instance.GetPhaseDatas(contentType, ref rule);

        StartPlay();
    }

    private void StartPlay()
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

        random = new System.Random();
        cardDrawer.Init(phaseDatas, random);

        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        isMyTurn = PhotonNetwork.IsMasterClient; // 마스터 클라이언트가 먼저 시작
        cardDrawer.DrawCard(isMyTurn, myCards, NextTurn);
    }

    private void InitCards(int cardCount, List<Card> cards, bool isPlayer1)
    {
        for (int i = 0; i < cardCount; i++)
        {
            cardDrawer.DrawCard(isPlayer1, cards, NextTurn, false);
        }
    }

    private void NextTurn(Card removedCard)
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
        cardDrawer.DrawCard(isMyTurn, isMyTurn ? myCards : otherCards, NextTurn);

        //만약 상대 턴이면, 상대가 둘 수 있도록 한다.
        if (!isMyTurn)
        {
            photonView.RPC("RPC_StartPlaceCard", RpcTarget.Others, UnityEngine.Random.Range(1f, 4f));
        }
    }

    [PunRPC]
    private void RPC_StartPlaceCard(float delay)
    {
        StartCoroutine(DelayPlaceCard(delay));
    }

    private IEnumerator DelayPlaceCard(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 상대 플레이어가 카드를 놓는 로직을 구현합니다.
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

    public void UpdateMyScore(int score)
    {
        myScore += score;
        gameUI.UpdateMyScore(myScore);
    }

    public void UpdateOtherScore(int score)
    {
        otherScore += score;
        gameUI.UpdateOtherScore(otherScore);
    }

    #endregion
}