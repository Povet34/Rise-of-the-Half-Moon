using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PVEGameManager : GameManager
{
    public class GameInitData
    {
        public PhaseData.ContentType contentType;
        public int initBotLevel;
    }

    List<PhaseData> phaseDatas;
    BotLevelData initBotLevelData;

    NodeGenerator nodeGenerator;

    [Header("Me")]
    int myScore;
    List<Card> myCards = new List<Card>();

    [Header("Other")]
    [SerializeField] Bot botPrefab;
    Bot bot;
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
        initBotLevelData = ContentsDataManager.Instance.GetBotLevelData(initData.initBotLevel);

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


        bot = Instantiate(botPrefab);
        bot.Init(initBotLevelData, otherCards);

        isMyTurn = true; // �÷��̾ ���� ����
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

        //���� ai ���̸�, bot�� �� �� �ֵ��� �Ѵ�.
        if (!isMyTurn)
        {
            bot.StartPlaceCard(UnityEngine.Random.Range(1f, 4f));
        }
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