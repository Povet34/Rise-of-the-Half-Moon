using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] int defficulity = -1; // Bot의 난이도
    [SerializeField] int initRandomCount;
    private int randomCount;
    public List<Card> cards; // Bot이 가지고 있는 카드 리스트
    private NodeGenerator nodeGenerator;

    public void Init(List<Card> cards)
    {
        randomCount = initRandomCount;
        this.cards = cards;
        nodeGenerator = FindObjectOfType<NodeGenerator>();
    }

    private void PlaceCardByPriority()
    {
        List<Node> emptyNodes = nodeGenerator.FindEmptyOccupidNodes();
        if (cards.Count == 0 || emptyNodes.Count == 0) return;

        Card cardToPlace = null;
        Node targetNode = null;
        int highestScore = int.MinValue;

        // 각 카드와 노드 조합의 점수를 계산하여 가장 높은 점수를 가진 조합을 선택
        foreach (Card card in cards)
        {
            foreach (Node node in emptyNodes)
            {
                int score = RuleManager.Instance.PredictScoreForCard(node, card) + BotHandicap();
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

        // 임의의 카드와 Node 선택
        Card cardToPlace = cards[Random.Range(0, cards.Count)];
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
        yield return new WaitUntil(()=> !RuleManager.Instance.IsRemainScoreSettlement());
        yield return new WaitForSeconds(delay);

        if (defficulity == -1 || randomCount > 0)
        {
            PlaceCardRandom();
            randomCount--;
        }
        else
            PlaceCardByPriority();
    }

    private int BotHandicap()
    {
        return Random.Range(0, defficulity);
    }
}
