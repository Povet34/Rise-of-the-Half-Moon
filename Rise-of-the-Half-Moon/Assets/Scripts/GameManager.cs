using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Data")]
    public GameObject cardPrefab;
    public Transform canvasTransform; 
    public MoonPhaseData[] moonPhaseDataArray;
    private System.Random random;
    private GridGeneratorWithEightDirections gridGenerator;
    private bool isPlayerTurn;

    [Header("Me")]
    public List<Card> myCards;
    
    private static readonly Vector2[] myTwoCardPositions = {
        new Vector2(-80, -400),
        new Vector2(80, -400)
    };
    private static readonly Vector2[] myThreeCardPositions = {
        new Vector2(-150, -400),
        new Vector2(0, -400),
        new Vector2(150, -400)
    };

    [Header("Other")]
    public Bot bot;
    public List<Card> otherCards;

    private static readonly Vector2[] otherTwoCardPositions = {
        new Vector2(-80, 400),
        new Vector2(80, 400)
    };
    private static readonly Vector2[] otherThreeCardPositions = {
        new Vector2(-150, 400),
        new Vector2(0, 400),
        new Vector2(150, 400)
    };


    private void Start()
    {
        gridGenerator = FindObjectOfType<GridGeneratorWithEightDirections>();
        gridGenerator.Create();

        random = new System.Random();
        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);

        bot.Init(otherCards, gridGenerator.Nodes);

        isPlayerTurn = true; // 플레이어가 먼저 시작
        DrawCard(isPlayerTurn);
    }

    private void InitCards(int cardCount, List<Card> cards, bool isPlayer1)
    {
        Vector2[] positions = GetCardPositions(cardCount, isPlayer1);

        for (int i = 0; i < cardCount; i++)
        {
            GameObject go = Instantiate(cardPrefab, canvasTransform);
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = positions[i];

            Card card = go.GetComponent<Card>();
            card.moonPhaseData = moonPhaseDataArray[random.Next(moonPhaseDataArray.Length)];
            card.isMine = isPlayer1;

            cards.Add(card);
        }
    }

    private Vector2[] GetCardPositions(int cardCount, bool isPlayer1)
    {
        if (isPlayer1)
        {
            return cardCount == 2 ? myTwoCardPositions : myThreeCardPositions;
        }
        else
        {
            return cardCount == 2 ? otherTwoCardPositions : otherThreeCardPositions;
        }
    }

    private void NextTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        DrawCard(isPlayerTurn);

        if (!isPlayerTurn)
        {
            bot.PlaceCard();
        }
    }

    private void DrawCard(bool isPlayerTurn)
    {
        List<Card> targetCards = isPlayerTurn ? myCards : otherCards;
        Vector2[] positions = GetCardPositions(targetCards.Count + 1, isPlayerTurn);

        GameObject go = Instantiate(cardPrefab, canvasTransform);
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = positions[targetCards.Count];

        Card card = go.GetComponent<Card>();
        card.moonPhaseData = moonPhaseDataArray[random.Next(moonPhaseDataArray.Length)];
        card.isMine = isPlayerTurn;
        card.SetCallbacks(NextTurn);

        targetCards.Add(card);
    }
}