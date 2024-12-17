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

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject punCardPrefab;

    [SerializeField] private Transform myCardArea;
    [SerializeField] private Transform otherCardArea;
    
    private List<PhaseData> phaseDatas;
    private System.Random random;
    private NodeGenerator nodeGenerator;

    public void Init(List<PhaseData> phaseDatas, System.Random random)
    {
        this.phaseDatas = phaseDatas;
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

        GameObject go = Instantiate(GameManager.Instance.IsNetworkGame ? punCardPrefab : cardPrefab, isPlayerTurn ? myCardArea : otherCardArea);
        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if (isTween)
            rectTransform.anchoredPosition = spawnPos;
        else
            rectTransform.anchoredPosition = positions[targetCards.Count];

        Card card = go.GetComponent<Card>();
        card.phaseData = phaseDatas[random.Next(phaseDatas.Count)];
        card.isMine = isPlayerTurn;
        targetCards.Add(card);

        card.Init(nextTurnCallback, 
            () => 
            {
                RepositionCards(targetCards, positions);
            },
            ()=>
            {
                foreach (var node in nodeGenerator.Nodes)
                {
                    node.UpdatePointValue(GameManager.Instance.Rule.PredictScoreForCard(node, card));
                }
            });

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


