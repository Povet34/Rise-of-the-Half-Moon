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
        // 초기화
        notifierRectTransform.sizeDelta = new Vector2(0, notifierRectTransform.sizeDelta.y);
        notifierText.text = "";
        notifierText.gameObject.SetActive(false);

        // width를 0에서 600으로 서서히 늘리기
        notifierRectTransform.DOSizeDelta(new Vector2(600, notifierRectTransform.sizeDelta.y), 1f).OnComplete(() =>
        {
            // text를 보여주기
            notifierText.text = text;
            notifierText.gameObject.SetActive(true);

            // 1초 뒤에 text가 사라지고 width가 다시 0으로 줄어들기
            DOVirtual.DelayedCall(1f, () =>
            {
                notifierText.gameObject.SetActive(false);
                notifierRectTransform.DOSizeDelta(new Vector2(0, notifierRectTransform.sizeDelta.y), 1f);
            });
        });
    }
}
