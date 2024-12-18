using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PUNCardDrawer : MonoBehaviourPun, ICardDrawer
{
    public static bool isDrawing;
    GameManager gameManager;

    [SerializeField] private GameObject punCardPrefab;

    private Transform myCardArea;
    private Transform otherCardArea;

    private List<PhaseData> phaseDatas;
    private NodeGenerator nodeGenerator;

    private List<ICard> myCards;
    private List<ICard> otherCards;
    private Action<ICard> nextTurnCallback;

    public void Init(List<PhaseData> phaseDatas, ref List<ICard> myCards, ref List<ICard> otherCards, Action<ICard> nextTurnCallback)
    {
        this.phaseDatas = phaseDatas;

        this.myCards = myCards;
        this.otherCards = otherCards;
        this.nextTurnCallback = nextTurnCallback;

        gameManager = FindAnyObjectByType<GameManager>();
        nodeGenerator = FindObjectOfType<NodeGenerator>();

        CardArea[] areas = FindObjectsOfType<CardArea>();
        foreach (var area in areas)
        {
            if (area.playerIndex == 1)
            {
                myCardArea = area.transform;
            }
            else if (area.playerIndex == 2)
            {
                otherCardArea = area.transform;
            }
        }
    }

    public void DrawCard(bool isPlayerTurn, bool isTween = true)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        List<ICard> targetCards = isPlayerTurn ? myCards : otherCards;
        Vector2[] positions = GetCardPositions(targetCards.Count + 1, isPlayerTurn);
        Vector2 spawnPos = isPlayerTurn ? Definitions.MyDrawCardSpawnPos : Definitions.OhterDrawCardSpawnPos;

        if (targetCards.Count >= positions.Length)
        {
            Debug.LogError("Not enough positions defined for the number of cards.");
            return;
        }

        GameObject go = null;
        if (gameManager.IsNetworkGame)
        {
            go = PhotonNetwork.Instantiate(Definitions.PUNCard, spawnPos, Quaternion.identity);
            go.transform.SetParent(isPlayerTurn ? myCardArea : otherCardArea, false);

            ICard card = go.GetComponent<ICard>();
            card.phaseData = phaseDatas[Random.Range(0, phaseDatas.Count)];
            card.IsMine = isPlayerTurn;

            photonView.RPC(nameof(RPC_DrawCard), RpcTarget.Others, isPlayerTurn, targetCards.Count, spawnPos, isTween, (int)card.phaseData.contentType, card.phaseData.phaseIndex);
        }

        InitializeCard(go, isPlayerTurn, isTween, positions);
    }

    [PunRPC]
    private void RPC_DrawCard(bool isPlayerTurn, int cardIndex, Vector2 spawnPos, bool isTween, int contentType, int phaseIndex)
    {
        List<ICard> targetCards = !isPlayerTurn ? myCards : otherCards;
        Vector2[] positions = GetCardPositions(cardIndex + 1, !isPlayerTurn);

        GameObject go = PhotonNetwork.Instantiate(Definitions.PUNCard, spawnPos, Quaternion.identity);
        go.transform.SetParent(!isPlayerTurn ? myCardArea : otherCardArea, false);

        PhaseData phaseData = ContentsDataManager.Instance.GetPhaseData((PhaseData.ContentType)contentType, phaseIndex);
        InitializeCard(go, !isPlayerTurn, isTween, positions, cardIndex, phaseData);
    }

    private void InitializeCard(GameObject go, bool isPlayerTurn, bool isTween, Vector2[] positions, int cardIndex = -1, PhaseData phaseData = null)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if (isTween)
            rectTransform.anchoredPosition = positions[cardIndex == -1 ? (isPlayerTurn ? myCards.Count : otherCards.Count) : cardIndex];
        else
            rectTransform.anchoredPosition = positions[cardIndex == -1 ? (isPlayerTurn ? myCards.Count : otherCards.Count) : cardIndex];

        ICard card = go.GetComponent<ICard>();
        card.phaseData = phaseData;
        card.IsMine = isPlayerTurn;

        List<ICard> targetCards = isPlayerTurn ? myCards : otherCards;
        targetCards.Add(card);

        if (nextTurnCallback != null)
        {
            ICard.CardParam param = new ICard.CardParam();
            param.nextTurnCallback = nextTurnCallback;
            param.replaceCallback = () => { RepositionCards(targetCards, positions); };
            param.selectCallback = () =>
            {
                foreach (var node in nodeGenerator.Nodes)
                {
                    node.UpdatePointValue(gameManager.Rule.PredictScoreForCard(node, card));
                }
            };

            card.Init(param);
        }

        //Draw Animation
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => { isDrawing = true; });
            sequence.Append(rectTransform.DOAnchorPos(positions[cardIndex == -1 ? targetCards.Count - 1 : cardIndex], Definitions.CardMoveDuration).SetEase(Ease.OutQuint));
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