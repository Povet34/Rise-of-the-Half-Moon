using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    private GameObject cardPrefab;
    private Transform canvasTransform;
    private MoonPhaseData[] moonPhaseDataArray;
    private System.Random random;

    public void Init(GameObject cardPrefab, Transform canvasTransform, MoonPhaseData[] moonPhaseDataArray, System.Random random)
    {
        this.cardPrefab = cardPrefab;
        this.canvasTransform = canvasTransform;
        this.moonPhaseDataArray = moonPhaseDataArray;
        this.random = random;
    }

    public void DrawCard(bool isPlayerTurn, List<Card> targetCards, System.Action<Card> nextTurnCallback)
    {
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
        card.SetCallbacks(nextTurnCallback);

        targetCards.Add(card);

        RepositionCards(targetCards, positions);
    }

    private void RepositionCards(List<Card> cards, Vector2[] positions)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform rectTransform = cards[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = positions[i];
        }
    }

    private Vector2[] GetCardPositions(int cardCount, bool isPlayer1)
    {
        if (isPlayer1)
            return cardCount == 2 ? Definitions.MyTwoCardPositions : Definitions.MyThreeCardPositions;
        else
            return cardCount == 2 ? Definitions.OtherTwoCardPositions : Definitions.OtherThreeCardPositions;
    }
}


