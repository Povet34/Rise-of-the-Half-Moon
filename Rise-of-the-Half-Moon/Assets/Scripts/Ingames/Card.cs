using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ICard
{
    public static bool IsDraging { get; protected set; }
    public bool IsMine { get; set; }
    public PhaseData phaseData { get; set; }
    public RectTransform rt { get; set; }

    private CanvasGroup canvasGroup;
    GameManager gameManager;

    [SerializeField] private Image cardImage;

    Action<Card> nextTurnCallback;
    Action replaceCallback;
    Action selectCallback;

    Vector2 dragPos;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
    }

    public void Init(ICard.CardParam cardParam)
    {
        gameManager = FindAnyObjectByType<GameManager>();

        nextTurnCallback = cardParam.nextTurnCallback;
        replaceCallback = cardParam.replaceCallback;
        selectCallback = cardParam.selectCallback;

        if (phaseData != null)
        {
            cardImage.sprite = phaseData.GetSprite(IsMine);
        }
    }

    #region Player Input

    private bool CanInput()
    {
        return IsMine && !gameManager.Rule.IsRemainScoreSettlement() && !PUNCardDrawer.isDrawing && gameManager.isMyTurn;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!CanInput())
            return; 

        dragPos = transform.position;
        IsDraging = true;

        canvasGroup.alpha = 0.6f; // �巡�� �� ���� ����
        canvasGroup.blocksRaycasts = false; // �巡�� �� �ٸ� UI ��ҿ��� ��ȣ�ۿ� ���
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanInput())
            return;

        rt.anchoredPosition += eventData.delta / canvasGroup.transform.localScale.x; // �巡�� ��ġ ������Ʈ

        selectCallback?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanInput())
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
                if(node.occupiedUser == Definitions.EMPTY_NODE)
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

        IsDraging = false; 
    }

    #endregion

    public void PlaceCard(Node node)
    {
        Node.PutData data = new Node.PutData();
        data.occupiedUser = Definitions.NOT_OCCUPIED_NODE;
        data.moonPhaseData = phaseData;

        node.PutCard(data);
        gameManager.Rule.OnCardPlaced(node, IsMine);

        nextTurnCallback?.Invoke(this);
        replaceCallback?.Invoke();

        Destroy();
    }

    public void Destroy()
    {
        IsDraging = false;
        Destroy(gameObject);
    }
}