using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CardDrawer : MonoBehaviour
{
    /// <summary>
    /// Draw중인지 아닌지 판단
    /// </summary>
    public static bool isDrawing;

    private GameObject cardPrefab;
    private Transform canvasTransform;
    private MoonPhaseData[] moonPhaseDataArray;
    private System.Random random;
    private NodeGenerator nodeGenerator;

    public void Init(GameObject cardPrefab, Transform canvasTransform, MoonPhaseData[] moonPhaseDataArray, System.Random random)
    {
        this.cardPrefab = cardPrefab;
        this.canvasTransform = canvasTransform;
        this.moonPhaseDataArray = moonPhaseDataArray;
        this.random = random;

        nodeGenerator = FindObjectOfType<NodeGenerator>();
    }

    public void DrawCard(bool isPlayerTurn, List<Card> targetCards, System.Action<Card> nextTurnCallback, bool isTween = true)
    {
        Vector2[] positions = GetCardPositions(targetCards.Count + 1, isPlayerTurn);
        Vector2 spawnPos = isPlayerTurn ? Definitions.MyDrawCardSpawnPos : Definitions.OhterDrawCardSpawnPos;

        if (targetCards.Count >= positions.Length)
        {
            Debug.LogError("Not enough positions defined for the number of cards.");
            return;
        }

        GameObject go = Instantiate(cardPrefab, canvasTransform);
        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if (isTween)
            rectTransform.anchoredPosition = spawnPos;
        else
            rectTransform.anchoredPosition = positions[targetCards.Count];

        Card card = go.GetComponent<Card>();
        card.moonPhaseData = moonPhaseDataArray[random.Next(moonPhaseDataArray.Length)];
        card.isMine = isPlayerTurn;

        card.Init(nextTurnCallback, 
            () => 
            {
                RepositionCards(targetCards, positions);
            },
            ()=>
            {
                foreach (var node in nodeGenerator.Nodes)
                {
                    node.UpdatePointValue(RuleManager.Instance.PredictScoreForCard(node, card));
                }
            });

        targetCards.Add(card);


        //Draw Animation
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => { isDrawing = true; });
            sequence.AppendCallback(() => { card.GetComponent<RectTransform>().DOAnchorPos(positions[targetCards.Count - 1], Definitions.CardMoveDuration).SetEase(Ease.OutQuint); });
            sequence.AppendCallback(() => { isDrawing = false; });
            sequence.AppendCallback(() => { RepositionCards(targetCards, positions); });
        }
    }

    private void RepositionCards(List<Card> cards, Vector2[] positions)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform rectTransform = cards[i].GetComponent<RectTransform>();
            rectTransform.DOAnchorPos(positions[i], Definitions.CardMoveDuration).SetEase(Ease.OutQuint);
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


