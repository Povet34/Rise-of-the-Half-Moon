using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public List<Card> cards; // Bot이 가지고 있는 카드 리스트
    public List<Node> nodes; // 가능한 Node 리스트

    public void Init(List<Card> cards, List<Node> nodes)
    {
        this.cards = cards;
        this.nodes = nodes;
    }

    public void PlaceCard()
    {
        if (cards.Count == 0 || nodes.Count == 0) return;

        // 임의의 카드와 Node 선택
        Card cardToPlace = cards[Random.Range(0, cards.Count)];
        Node targetNode = nodes[Random.Range(0, nodes.Count)];

        cards.Remove(cardToPlace);
        cardToPlace.PlaceCard(targetNode);
    }
}
