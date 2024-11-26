using System.Collections.Generic;
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
        for (int i = 0; i < cardCount; i++)
        {
            DrawCard(isPlayer1, cards);
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

    private void NextTurn(Card removedCard)
    {
        //리스트에서 제거하고
        if(isPlayerTurn)
            myCards.Remove(removedCard); 
        else
            otherCards.Remove(removedCard);

        //턴을 바꿔주고
        isPlayerTurn = !isPlayerTurn;

        //드로우한다
        DrawCard(isPlayerTurn);

        //만약 ai 턴이면, bot이 둘 수 있도록 한다.
        if (!isPlayerTurn)
        {
            bot.PlaceCard();
        }
    }

    private void DrawCard(bool isPlayerTurn, List<Card> targetCards = null)
    {
        if (targetCards == null)
        {
            targetCards = isPlayerTurn ? myCards : otherCards;
        }

        Vector2[] positions = GetCardPositions(targetCards.Count + 1, isPlayerTurn);

        if (targetCards.Count >= positions.Length)
        {
            Debug.LogError("Not enough positions defined for the number of cards.");
            return;
        }

        GameObject go = Instantiate(cardPrefab, canvasTransform);
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = positions[targetCards.Count];

        Card card = go.GetComponent<Card>();
        card.moonPhaseData = moonPhaseDataArray[random.Next(moonPhaseDataArray.Length)];
        card.isMine = isPlayerTurn;
        card.SetCallbacks(NextTurn);

        targetCards.Add(card);

        RepositionCards(targetCards, isPlayerTurn);
    }

    private void RepositionCards(List<Card> cards, bool isPlayerTurn)
    {
        Vector2[] positions = GetCardPositions(cards.Count, isPlayerTurn);

        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform rectTransform = cards[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = positions[i];
        }
    }
}