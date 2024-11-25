using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab; // Card 프리팹을 참조
    public Transform canvasTransform; // Canvas Transform을 참조
    public MoonPhaseData[] moonPhaseDataArray; // MoonPhaseData 배열을 참조

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
        CreateCards(2); // 카드 2개 생성
        // CreateCards(3); // 카드 3개 생성 (필요 시 주석 해제)
    }

    private void CreateCards(int cardCount)
    {
        Vector2[] positions = cardCount == 2 ? twoCardPositions : threeCardPositions;

        for (int i = 0; i < cardCount; i++)
        {
            // 카드 생성
            GameObject card = Instantiate(cardPrefab, canvasTransform);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = positions[i]; // 고정된 위치 설정
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // 카드에 MoonPhaseData 랜덤 할당
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.moonPhaseData = moonPhaseDataArray[Random.Range(0, moonPhaseDataArray.Length)];
        }
    }
}