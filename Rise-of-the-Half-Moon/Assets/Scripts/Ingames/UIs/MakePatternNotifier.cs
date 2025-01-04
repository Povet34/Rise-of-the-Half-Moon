using DG.Tweening;
using TMPro;
using UnityEngine;

public class MakePatternNotifier : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notifierText;
    RectTransform notifierRectTransform;

    public void ShowNotifier(string text)
    {
        if(!notifierRectTransform)
            notifierRectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(true);

        // 초기화
        notifierRectTransform.sizeDelta = new Vector2(0, notifierRectTransform.sizeDelta.y);
        notifierText.text = "";
        notifierText.gameObject.SetActive(false);

        // 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // width를 0에서 600으로 서서히 늘리기
        sequence.Append(notifierRectTransform.DOSizeDelta(new Vector2(600, notifierRectTransform.sizeDelta.y), 0.2f));

        // text를 보여주기
        sequence.AppendCallback(() =>
        {
            notifierText.text = text;
            notifierText.gameObject.SetActive(true);
        });

        // 1초 대기
        sequence.AppendInterval(0.2f);

        // text가 사라지고 width가 다시 0으로 줄어들기
        sequence.AppendCallback(() =>
        {
            notifierText.gameObject.SetActive(false);
        });
        sequence.Append(notifierRectTransform.DOSizeDelta(new Vector2(0, notifierRectTransform.sizeDelta.y), 0.2f));
        sequence.onComplete += () =>
        {
            notifierText.text = "";
            gameObject.SetActive(false);
        };
    }
}
