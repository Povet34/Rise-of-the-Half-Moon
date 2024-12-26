using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardDrawer : MonoBehaviour, ICardDrawer
{
    public static bool isDrawing;
    GameManager gameManager;

    [SerializeField] private GameObject cardPrefab;

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
        List<ICard> targetCards = isPlayerTurn ? myCards : otherCards;
        Vector2[] positions = GetCardPositions(targetCards.Count + 1);
        Vector2 spawnPos = isPlayerTurn ? Definitions.MyDrawCardSpawnPos : Definitions.OhterDrawCardSpawnPos;

        if (targetCards.Count >= positions.Length)
        {
            Debug.LogError("Not enough positions defined for the number of cards.");
            return;
        }

        GameObject go = Instantiate(cardPrefab, spawnPos, Quaternion.identity);
        go.transform.SetParent(isPlayerTurn ? myCardArea : otherCardArea, false);

        ICard card = go.GetComponent<ICard>();
        card.phaseData = phaseDatas[Random.Range(0, phaseDatas.Count)];
        card.IsMine = isPlayerTurn;

        InitializeCard(go, isPlayerTurn, isTween, positions, card.phaseData);
    }

    private void InitializeCard(GameObject go, bool isPlayerTurn, bool isTween, Vector2[] positions, PhaseData phaseData = null)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if (isTween)
            rectTransform.anchoredPosition = isPlayerTurn ? Definitions.MyDrawCardSpawnPos : Definitions.OhterDrawCardSpawnPos;
        else
            rectTransform.anchoredPosition = positions[isPlayerTurn ? myCards.Count : otherCards.Count];

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

            card.Init(param);
        }

        //Draw Animation
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => { isDrawing = true; });
            sequence.AppendCallback(() => { RepositionCards(targetCards, positions); });
            sequence.Append(rectTransform.DOAnchorPos(positions[targetCards.Count - 1], Definitions.CardMoveDuration).SetEase(Ease.OutQuint));
            sequence.AppendCallback(() => { isDrawing = false; });

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

    Vector2[] GetCardPositions(int cardCount)
    {
        switch (cardCount)
        {
            case 1:
                return Definitions.TwoCardPositions;
            case 2:
                return Definitions.ThreeCardPositions;
            case 3:
                return Definitions.FourCardPositions;
            default:
                return null;
        }
    }
}