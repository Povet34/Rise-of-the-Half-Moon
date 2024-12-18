using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    int accuracy = -1; // Bot�� ���̵�
    int initRandomPutCount;
    int putCount;
    NodeGenerator nodeGenerator;
    GameManager gameManager;

    public List<ICard> cards; // Bot�� ������ �ִ� ī�� ����Ʈ

    public void Init(BotLevelData levelData, List<ICard> cards)
    {
        putCount = initRandomPutCount;
        nodeGenerator = FindObjectOfType<NodeGenerator>();
        gameManager = FindAnyObjectByType<GameManager>();

        accuracy = levelData.accuracy;
        initRandomPutCount = levelData.initRandomPutCount;

        this.cards = cards;
    }

    private void PlaceCardByPriority()
    {
        List<Node> emptyNodes = nodeGenerator.FindEmptyOccupidNodes();
        if (cards.Count == 0 || emptyNodes.Count == 0) return;

        ICard cardToPlace = null;
        Node targetNode = null;
        int highestScore = int.MinValue;

        // �� ī��� ��� ������ ������ ����Ͽ� ���� ���� ������ ���� ������ ����
        foreach (ICard card in cards)
        {
            foreach (Node node in emptyNodes)
            {
                int score = gameManager.Rule.PredictScoreForCard(node, card) + BotHandicap();
                if (score > highestScore)
                {
                    highestScore = score;
                    cardToPlace = card;
                    targetNode = node;
                }
            }
        }

        if (cardToPlace == null || targetNode == null)
        {
            PlaceCardRandom();
            return;
        }

        cards.Remove(cardToPlace);
        cardToPlace.PlaceCard(targetNode);
    }

    private void PlaceCardRandom()
    {
        List<Node> emptyNodes = nodeGenerator.FindEmptyOccupidNodes();

        if (cards.Count == 0 || emptyNodes.Count == 0) return;

        // ������ ī��� Node ����
        ICard cardToPlace = cards[Random.Range(0, cards.Count)];
        Node targetNode = emptyNodes[Random.Range(0, emptyNodes.Count)];

        cards.Remove(cardToPlace);
        cardToPlace.PlaceCard(targetNode);
    }

    public void StartPlaceCard(float delay)
    {
        StartCoroutine(DelayPlaceCardBody(delay));
    }

    private IEnumerator DelayPlaceCardBody(float delay)
    {
        yield return new WaitUntil(()=> !gameManager.Rule.IsRemainScoreSettlement());
        yield return new WaitForSeconds(delay);

        if (accuracy == -1 || putCount > initRandomPutCount)
        {
            PlaceCardRandom();
            putCount++;
        }
        else
            PlaceCardByPriority();
    }

    private int BotHandicap()
    {
        return Random.Range(0, accuracy);
    }
}
