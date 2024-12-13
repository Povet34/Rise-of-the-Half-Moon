using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PVEGameManager : VolatilitySingleton<PVEGameManager>
{
    public class GameInitData
    {
        public PhaseData.ContentType contentType;
        public int initBotLevel;
    }

    [Header("Data")]
    public GameObject cardPrefab;
    public Transform canvasTransform;
    public bool isPlayerTurn;
    public PhaseData.ContentType contentType;

    private ContentRule rule;
    public ContentRule Rule => rule;

    private List<PhaseData> phaseDatas;
    private BotLevelData initBotLevelData;

    private System.Random random;
    private NodeGenerator nodeGenerator;

    [Header("Me")]
    public int myScore;
    public List<Card> myCards;

    [Header("Other")]
    public Bot bot;
    public int otherScore;
    public List<Card> otherCards;

    private PVEGameUI pveGameUI;
    private CardDrawer cardDrawer;

    private void Awake()
    {
        nodeGenerator = FindObjectOfType<NodeGenerator>();
        pveGameUI.AddComponent<PVEGameUI>();
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
        cardDrawer = gameObject.AddComponent<CardDrawer>();
        cardDrawer.Init(cardPrefab, canvasTransform, phaseDatas, random);

        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        bot.Init(initBotLevelData, otherCards);

        isPlayerTurn = true; // 플레이어가 먼저 시작
        cardDrawer.DrawCard(isPlayerTurn, myCards, NextTurn);
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
        if (isPlayerTurn)
            myCards.Remove(removedCard);
        else
            otherCards.Remove(removedCard);

        //턴을 바꿔주고
        isPlayerTurn = !isPlayerTurn;

        //드로우한다
        cardDrawer.DrawCard(isPlayerTurn, isPlayerTurn ? myCards : otherCards, NextTurn);

        //만약 ai 턴이면, bot이 둘 수 있도록 한다.
        if (!isPlayerTurn)
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
                    pveGameUI.ShowWin();
                }
                else if (myScore < otherScore)
                {
                    pveGameUI.ShowLose();
                }
                else
                {
                    pveGameUI.ShowDraw();
                }
            });
    }

    #region Update Score

    public void UpdateMyScore(int score)
    {
        myScore += score;
        pveGameUI.UpdateMyScore(myScore);
    }

    public void UpdateOtherScore(int score)
    {
        otherScore += score;
        pveGameUI.UpdateOtherScore(otherScore);
    }

    #endregion
}