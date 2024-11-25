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

    public void PlaceCard()
    {
        if (cards.Count == 0 || nodes.Count == 0) return;

        // ������ ī��� Node ����
        Card cardToPlace = cards[Random.Range(0, cards.Count)];
        Node targetNode = nodes[Random.Range(0, nodes.Count)];

        cards.Remove(cardToPlace);
        cardToPlace.PlaceCard(targetNode);
    }
}
