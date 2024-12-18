using DG.Tweening;
using Photon.Pun;
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

    public void DrawCard(bool isPlayerTurn, List<ICard> targetCards, System.Action<ICard> nextTurnCallback, bool isTween = true)
    {
        Vector2[] positions = GetCardPositions(targetCards.Count + 1, isPlayerTurn);
        Vector2 spawnPos = isPlayerTurn ? Definitions.MyDrawCardSpawnPos : Definitions.OhterDrawCardSpawnPos;

        if (targetCards.Count >= positions.Length)
        {
            Debug.LogError("Not enough positions defined for the number of cards.");
            return;
        }

        GameObject go = null;
        if (GameManager.Instance.IsNetworkGame)
        {
            go = PhotonNetwork.Instantiate(punCardPrefab.name, spawnPos, punCardPrefab.transform.rotation);
            go.transform.SetParent(isPlayerTurn ? myCardArea : otherCardArea, false);
        }
        else
        {
            go = Instantiate(cardPrefab, isPlayerTurn ? myCardArea : otherCardArea);
        }

        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if (isTween)
            rectTransform.anchoredPosition = spawnPos;
        else
            rectTransform.anchoredPosition = positions[targetCards.Count];

        ICard card = go.GetComponent<ICard>();
        card.phaseData = phaseDatas[random.Next(phaseDatas.Count)];
        card.IsMine = isPlayerTurn;
        targetCards.Add(card);

        ICard.CardParam param = new ICard.CardParam();
        param.nextTurnCallback = nextTurnCallback;
        param.replaceCallback = () => { RepositionCards(targetCards, positions); };
        param.selectCallback = () =>
        {
            foreach (var node in nodeGenerator.Nodes)
            {
                node.UpdatePointValue(GameManager.Instance.Rule.PredictScoreForCard(node, card));
            }
        };

        card.Init(param);

        //Draw Animation
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => { isDrawing = true; });
            sequence.Append(rectTransform.DOAnchorPos(positions[targetCards.Count - 1], Definitions.CardMoveDuration).SetEase(Ease.OutQuint));
            sequence.AppendCallback(() => { isDrawing = false; });
            sequence.AppendCallback(() => { RepositionCards(targetCards, positions); });
        }
    }

    private void RepositionCards(List<ICard> cards, Vector2[] positions)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform rectTransform = cards[i].rt;
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