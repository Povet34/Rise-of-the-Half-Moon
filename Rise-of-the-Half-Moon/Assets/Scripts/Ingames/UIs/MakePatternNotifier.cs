using DG.Tweening;
using TMPro;
using UnityEngine;

public class MakePatternNotifier : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notifierText;
    RectTransform notifierRectTransform;

    private void Start()
    {
        notifierRectTransform = GetComponent<RectTransform>();
    }

    public void ShowNotifier(string text)
    {
        // �ʱ�ȭ
        notifierRectTransform.sizeDelta = new Vector2(0, notifierRectTransform.sizeDelta.y);
        notifierText.text = "";
        notifierText.gameObject.SetActive(false);

        // width�� 0���� 600���� ������ �ø���
        notifierRectTransform.DOSizeDelta(new Vector2(600, notifierRectTransform.sizeDelta.y), 1f).OnComplete(() =>
        {
            // text�� �����ֱ�
            notifierText.text = text;
            notifierText.gameObject.SetActive(true);

            // 1�� �ڿ� text�� ������� width�� �ٽ� 0���� �پ���
            DOVirtual.DelayedCall(1f, () =>
            {
                notifierText.gameObject.SetActive(false);
                notifierRectTransform.DOSizeDelta(new Vector2(0, notifierRectTransform.sizeDelta.y), 1f);
            });
        });
    }
}
