using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Data")]
    public GameObject cardPrefab;
    public Transform canvasTransform; 
    public MoonPhaseData[] moonPhaseDataArray;
    private System.Random random;

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
        random = new System.Random();
        InitCards(2, myCards, true);
        InitCards(2, otherCards, false);
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
}