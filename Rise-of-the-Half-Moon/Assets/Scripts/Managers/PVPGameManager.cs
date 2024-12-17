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

        isMyTurn = PhotonNetwork.IsMasterClient; // ������ Ŭ���̾�Ʈ�� ���� ����
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

        //����Ʈ���� �����ϰ�
        if (isMyTurn)
            myCards.Remove(removedCard);
        else
            otherCards.Remove(removedCard);

        //���� �ٲ��ְ�
        isMyTurn = !isMyTurn;

        //��ο��Ѵ�
        cardDrawer.DrawCard(isMyTurn, isMyTurn ? myCards : otherCards, NextTurn);

        //���� ��� ���̸�, ��밡 �� �� �ֵ��� �Ѵ�.
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
        // ��� �÷��̾ ī�带 ���� ������ �����մϴ�.
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