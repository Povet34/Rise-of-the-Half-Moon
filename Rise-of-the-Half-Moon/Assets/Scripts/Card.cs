using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isMine;
    public MoonPhaseData moonPhaseData; // MoonPhaseData�� ����
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [SerializeField] private Image cardImage;

    Action<Card> nextTurnCallback;
    Action replaceCallback;
    Action selectCallback;

    Vector2 dragPos;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (moonPhaseData != null)
        {
            cardImage.sprite = moonPhaseData.GetSprite(isMine);
            cardImage.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void Init(Action<Card> nextTurnCallback, Action replaceCallback, Action selectCallback)
    {
        this.nextTurnCallback = nextTurnCallback;
        this.replaceCallback = replaceCallback;
        this.selectCallback = selectCallback;
    }

    #region Player Input

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isMine && RuleManager.Instance.isAnimating)
            return;

        dragPos = transform.position;

        canvasGroup.alpha = 0.6f; // �巡�� �� ���� ����
        canvasGroup.blocksRaycasts = false; // �巡�� �� �ٸ� UI ��ҿ��� ��ȣ�ۿ� ���
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isMine && RuleManager.Instance.isAnimating)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvasGroup.transform.localScale.x; // �巡�� ��ġ ������Ʈ

        selectCallback?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isMine && RuleManager.Instance.isAnimating)
            return;

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
                if(node.occupiedUser == Definitions.NOT_OCCUPIED)
                {
                    PlaceCard(node);
                    return;
                }
                else
                {
                    node.transform.DOShakePosition(0.5f, 0.5f, 10, 90, false, true);
                    node.transform.DOShakeScale(0.5f, 0.5f, 10, 90, false);
                }
            }
        }

        transform.position = dragPos;
        dragPos = Vector2.zero;
    }

    #endregion

    public void PlaceCard(Node node)
    {
        Node.PutData data = new Node.PutData();
        data.occupiedUser = isMine ? Definitions.MY_INDEX : Definitions.OTHER_INDEX;
        data.moonPhaseData = moonPhaseData;

        node.PutCard(data);
        nextTurnCallback?.Invoke(this);
        replaceCallback?.Invoke();

        RuleManager.Instance.OnCardPlaced(node);

        Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}