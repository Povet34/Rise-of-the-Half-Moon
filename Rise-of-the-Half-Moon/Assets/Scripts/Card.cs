using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isMine;
    public PhaseData phaseData;
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

    public void Init(Action<Card> nextTurnCallback, Action replaceCallback, Action selectCallback)
    {
        this.nextTurnCallback = nextTurnCallback;
        this.replaceCallback = replaceCallback;
        this.selectCallback = selectCallback;

        if (phaseData != null)
        {
            cardImage.sprite = phaseData.GetSprite(isMine);
        }
    }

    #region Player Input

    private bool CanInput()
    {
        return isMine && !GameManager.Instance.Rule.IsRemainScoreSettlement() && !CardDrawer.isDrawing && GameManager.Instance.isPlayerTurn;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!CanInput())
            return; 

        dragPos = transform.position;

        canvasGroup.alpha = 0.6f; // 드래그 중 투명도 조절
        canvasGroup.blocksRaycasts = false; // 드래그 중 다른 UI 요소와의 상호작용 허용
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanInput())
            return;

        rectTransform.anchoredPosition += eventData.delta / canvasGroup.transform.localScale.x; // 드래그 위치 업데이트

        selectCallback?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanInput())
            return;

        canvasGroup.alpha = 1.0f; // 드래그 종료 후 투명도 복원
        canvasGroup.blocksRaycasts = true; // 드래그 종료 후 상호작용 복원

        // 드래그 종료 시 레이를 쏴서 3D 오브젝트인 Node 감지
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
    }

    #endregion

    public void PlaceCard(Node node)
    {
        Node.PutData data = new Node.PutData();
        data.occupiedUser = Definitions.NOT_OCCUPIED_NODE;
        data.moonPhaseData = phaseData;

        node.PutCard(data);
        GameManager.Instance.Rule.OnCardPlaced(node, isMine);

        nextTurnCallback?.Invoke(this);
        replaceCallback?.Invoke();

        Destroy();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}