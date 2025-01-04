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

        // ÃÊ±âÈ­
        notifierRectTransform.sizeDelta = new Vector2(0, notifierRectTransform.sizeDelta.y);
        notifierText.text = "";
        notifierText.gameObject.SetActive(false);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(notifierRectTransform.DOSizeDelta(new Vector2(600, notifierRectTransform.sizeDelta.y), 0.2f));

        sequence.AppendCallback(() =>
        {
            notifierText.text = text;
            notifierText.gameObject.SetActive(true);
        });

        sequence.AppendInterval(0.5f);

        sequence.AppendCallback(() =>{ notifierText.gameObject.SetActive(false); });

        sequence.Append(notifierRectTransform.DOSizeDelta(new Vector2(0, notifierRectTransform.sizeDelta.y), 0.2f));
        sequence.onComplete += () =>
        {
            notifierText.text = "";
            gameObject.SetActive(false);
        };
    }
}
