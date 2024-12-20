using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PUNCardDrawer : MonoBehaviourPun, ICardDrawer
{
    public static bool isDrawing;
    GameManager gameManager;

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
        nodeGenerator = FindAnyObjectByType<NodeGenerator>();

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

        GameObject go = PhotonNetwork.Instantiate(Definitions.PUNCard, spawnPos, Quaternion.identity);
        go.transform.SetParent(isPlayerTurn ? myCardArea : otherCardArea, false);

        ICard card = go.GetComponent<ICard>();
        card.phaseData = phaseDatas[Random.Range(0, phaseDatas.Count)];
        card.IsMine = isPlayerTurn;

        InitializeCard(go, isPlayerTurn, isTween, positions, card.phaseData);

        // RPC 호출하여 다른 클라이언트에 객체 정보 전달
        photonView.RPC(nameof(SyncCard), RpcTarget.Others, go.GetComponent<PhotonView>().ViewID, isPlayerTurn, isTween, positions, card.phaseData.phaseIndex);
    }

    [PunRPC]
    public void SyncCard(int viewID, bool isPlayerTurn, bool isTween, Vector2[] positions, int phaseIndex)
    {
        GameObject go = PhotonView.Find(viewID).gameObject;
        go.transform.SetParent(!isPlayerTurn ? myCardArea : otherCardArea, false);

        ICard card = go.GetComponent<ICard>();
        card.phaseData = phaseDatas[phaseIndex];
        card.IsMine = !isPlayerTurn;

        InitializeCard(go, !isPlayerTurn, isTween, positions, card.phaseData);
    }

    private void InitializeCard(GameObject go, bool isPlayerTurn, bool isTween, Vector2[] positions, PhaseData phaseData)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        int cardIndex = isPlayerTurn ? myCards.Count : otherCards.Count;
        if (cardIndex >= positions.Length)
        {
            Debug.LogError("Index out of range: cardIndex is greater than or equal to positions length.");
            return;
        }

        if (isTween)
            rectTransform.anchoredPosition = isPlayerTurn ? Definitions.MyDrawCardSpawnPos : Definitions.OhterDrawCardSpawnPos;
        else
            rectTransform.anchoredPosition = positions[cardIndex];

        ICard card = go.GetComponent<ICard>();

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

        _DrawAnimation();

        // Draw Animation
        void _DrawAnimation()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => { isDrawing = true; });
            sequence.AppendCallback(() => { RepositionCards(targetCards, positions); });
            sequence.Append(rectTransform.DOAnchorPos(positions[targetCards.Count - 1], Definitions.CardMoveDuration).SetEase(Ease.OutQuint));
            sequence.AppendCallback(() => { isDrawing = false; });

            sequence.Play();
        }
    }


    void RepositionCards(List<ICard> cards, Vector2[] positions)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform rectTransform = cards[i].rt;
            rectTransform.DOAnchorPos(positions[i], Definitions.CardMoveDuration).SetEase(Ease.OutQuint);
        }
    }

    Vector2[] GetCardPositions(int cardCount, bool isPlayer1)
    {
        if (isPlayer1)
            return cardCount == 2 ? Definitions.MyTwoCardPositions : Definitions.MyThreeCardPositions;
        else
            return cardCount == 2 ? Definitions.OtherTwoCardPositions : Definitions.OtherThreeCardPositions;
    }
}