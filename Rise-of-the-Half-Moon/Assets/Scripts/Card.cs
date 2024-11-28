using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isMine;
    public MoonPhaseData moonPhaseData; // MoonPhaseData를 참조
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [SerializeField] private Image cardImage;

    Action<Card> nextTurnCallback;
    Action replaceCallback;

    Vector2 dragPos;

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

    public void SetCallbacks(Action<Card> nextTurnCallback, Action replaceCallback)
    {
        this.nextTurnCallback = nextTurnCallback;
        this.replaceCallback = replaceCallback;
    }

    #region Player Input

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isMine)
            return;

        dragPos = transform.position;

        canvasGroup.alpha = 0.6f; // 드래그 중 투명도 조절
        canvasGroup.blocksRaycasts = false; // 드래그 중 다른 UI 요소와의 상호작용 허용
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isMine)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvasGroup.transform.localScale.x; // 드래그 위치 업데이트
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isMine)
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
                if(node.OccupiedUser == 0)
                {
                    PlaceCard(node);
                    return;
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