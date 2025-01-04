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

        // �ʱ�ȭ
        notifierRectTransform.sizeDelta = new Vector2(0, notifierRectTransform.sizeDelta.y);
        notifierText.text = "";
        notifierText.gameObject.SetActive(false);

        // ������ ����
        Sequence sequence = DOTween.Sequence();

        // width�� 0���� 600���� ������ �ø���
        sequence.Append(notifierRectTransform.DOSizeDelta(new Vector2(600, notifierRectTransform.sizeDelta.y), 0.2f));

        // text�� �����ֱ�
        sequence.AppendCallback(() =>
        {
            notifierText.text = text;
            notifierText.gameObject.SetActive(true);
        });

        // 1�� ���
        sequence.AppendInterval(0.2f);

        // text�� ������� width�� �ٽ� 0���� �پ���
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
