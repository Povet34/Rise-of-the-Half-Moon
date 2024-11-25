using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab; // Card �������� ����
    public Transform canvasTransform; // Canvas Transform�� ����
    public MoonPhaseData[] moonPhaseDataArray; // MoonPhaseData �迭�� ����

    private static readonly Vector2[] twoCardPositions = {
        new Vector2(-80, -400),
        new Vector2(80, -400)
    };

    private static readonly Vector2[] threeCardPositions = {
        new Vector2(-150, -400),
        new Vector2(0, -400),
        new Vector2(150, -400)
    };

    private void Start()
    {
        CreateCards(2); // ī�� 2�� ����
        // CreateCards(3); // ī�� 3�� ���� (�ʿ� �� �ּ� ����)
    }

    private void CreateCards(int cardCount)
    {
        Vector2[] positions = cardCount == 2 ? twoCardPositions : threeCardPositions;

        for (int i = 0; i < cardCount; i++)
        {
            // ī�� ����
            GameObject card = Instantiate(cardPrefab, canvasTransform);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = positions[i]; // ������ ��ġ ����
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // ī�忡 MoonPhaseData ���� �Ҵ�
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.moonPhaseData = moonPhaseDataArray[Random.Range(0, moonPhaseDataArray.Length)];
        }
    }
}