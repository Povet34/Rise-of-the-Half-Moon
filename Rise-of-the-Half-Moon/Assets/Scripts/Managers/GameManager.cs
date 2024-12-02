using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Data")]
    public GameObject cardPrefab;
    public Transform canvasTransform;
    public MoonPhaseData[] moonPhaseDataArray;
    public bool isPlayerTurn;

    private System.Random random;
    private NodeGenerator nodeGenerator;

    [Header("Me")]
    public int myScore;
    public List<Card> myCards;

    [Header("Other")]
    public Bot bot;
    public int otherScore;
    public List<Card> otherCards;

    private CardDrawer cardDrawer;

    private void Awake()
    {
        nodeGenerator = FindObjectOfType<NodeGenerator>();
    }

    private void Start()
    {
        StartPlay();
    }

    public void StartPlay()
    {
        foreach(var card in otherCards)
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
        cardDrawer.Init(cardPrefab, canvasTransform, moonPhaseDataArray, random);

        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        bot.Init(otherCards);

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
        RuleManager.Instance.SettlementOccupiedNodes(
            () => 
            {
                if (myScore > otherScore)
                {
                    UIManager.Instance.ShowWin();
                }
                else if (myScore < otherScore)
                {
                    UIManager.Instance.ShowLose();
                }
                else
                {
                    UIManager.Instance.ShowDraw();
                }
            });
    }

    #region Update Score

    public void UpdateMyScore(int score)
    {
        myScore += score;
        UIManager.Instance.UpdateMyScore(myScore);
    }

    public void UpdateOtherScore(int score)
    {
        otherScore += score;
        UIManager.Instance.UpdateOtherScore(otherScore);
    }

    #endregion
}