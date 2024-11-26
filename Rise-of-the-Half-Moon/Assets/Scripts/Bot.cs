using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public List<Card> cards; // Bot�� ������ �ִ� ī�� ����Ʈ
    public List<Node> nodes; // ������ Node ����Ʈ

    public void Init(List<Card> cards, List<Node> nodes)
    {
        this.cards = cards;
        this.nodes = nodes;
    }

    private void PlaceCardRandom()
    {
        if (cards.Count == 0 || nodes.Count == 0) return;

        // ��� ������ ��� ���͸�
        List<Node> availableNodes = nodes.FindAll(node => node.OccupiedUser == 0);
        if (availableNodes.Count == 0) return;

        // ������ ī��� Node ����
        Card cardToPlace = cards[Random.Range(0, cards.Count)];
        Node targetNode = availableNodes[Random.Range(0, availableNodes.Count)];

        cards.Remove(cardToPlace);
        cardToPlace.PlaceCard(targetNode);
    }

    public void StartPlaceCard(float delay)
    {
        StartCoroutine(DelayPlaceCardBody(delay));
    }

    private IEnumerator DelayPlaceCardBody(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaceCardRandom();
    }
}
