using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isMine;
    public MoonPhaseData moonPhaseData; // MoonPhaseData�� ����
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [SerializeField] private Image cardBackgroundIamge;
    [SerializeField] private Image cardImage;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (moonPhaseData != null)
        {
            cardImage.sprite = moonPhaseData.GetSprite(isMine);
            cardImage.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isMine)
            return;

        canvasGroup.alpha = 0.6f; // �巡�� �� ���� ����
        canvasGroup.blocksRaycasts = false; // �巡�� �� �ٸ� UI ��ҿ��� ��ȣ�ۿ� ���
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvasGroup.transform.localScale.x; // �巡�� ��ġ ������Ʈ
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f; // �巡�� ���� �� ���� ����
        canvasGroup.blocksRaycasts = true; // �巡�� ���� �� ��ȣ�ۿ� ����

        // �巡�� ���� �� ���̸� ���� 3D ������Ʈ�� Node ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Node node = hit.collider.GetComponent<Node>();
            if (node != null)
            {
                node.PutCard(moonPhaseData);
                Destroy();
            }
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}