using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class PUNCard : MonoBehaviourPunCallbacks, IBeginDragHandler, IDragHandler, IEndDragHandler, IPunObservable, ICard
{
    public static bool IsDraging { get; private set; }
    public bool IsMine { get; set; }
    public PhaseData phaseData { get; set; }
    public RectTransform rt { get; set; }

    private CanvasGroup canvasGroup;
    private GameManager gameManager;

    [SerializeField] private Image cardImage;

    Action<PUNCard> nextTurnCallback;
    Action replaceCallback;
    Action selectCallback;
    Vector2 dragPos;

    public void Init(ICard.CardParam param)
    {
        gameManager = FindAnyObjectByType<GameManager>();
        canvasGroup = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();

        nextTurnCallback = param.nextTurnCallback;
        replaceCallback = param.replaceCallback;
        selectCallback = param.selectCallback;

        if (phaseData != null)
        {
            cardImage.sprite = phaseData.GetSprite(IsMine);
        }
    }

    #region Player Input

    private bool CanInput()
    {
        return IsMine && !gameManager.Rule.IsRemainScoreSettlement() && !CardDrawer.isDrawing && gameManager.isMyTurn;
    }   

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanInput())
            return;

        IsDraging = true;

        dragPos = transform.position;

        canvasGroup.alpha = 0.6f; // 드래그 중 투명도 조절
        canvasGroup.blocksRaycasts = false; // 드래그 중 다른 UI 요소와의 상호작용 허용
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanInput())
            return;

        rt.anchoredPosition += eventData.delta / canvasGroup.transform.localScale.x; // 드래그 위치 업데이트

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
                if (node.occupiedUser == Definitions.EMPTY_NODE)
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
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_PlaceCard), RpcTarget.All, node.index);
        }
        else
        {
            ExecutePlaceCard(node);
        }
    }

    [PunRPC]
    private void RPC_PlaceCard(int nodeIndex)
    {
        Node node = FindNodeByIndex(nodeIndex);
        if (node != null)
        {
            ExecutePlaceCard(node);
        }
        else
        {
            Debug.LogError("Node not found for index: " + nodeIndex);
        }
    }

    private void ExecutePlaceCard(Node node)
    {
        if (node == null)
        {
            Debug.LogError("Node is null in ExecutePlaceCard");
            return;
        }

        Node.PutData data = new Node.PutData();
        data.occupiedUser = Definitions.NOT_OCCUPIED_NODE;
        data.moonPhaseData = phaseData;

        node.PutCard(data);
        gameManager.Rule.OnCardPlaced(node, IsMine);

        nextTurnCallback?.Invoke(this);
        replaceCallback?.Invoke();

        Destroy();
    }

    private Node FindNodeByIndex(int index)
    {
        // NodeGenerator에서 노드를 찾는 로직을 구현합니다.
        NodeGenerator nodeGenerator = FindObjectOfType<NodeGenerator>();
        if (nodeGenerator == null)
        {
            Debug.LogError("NodeGenerator not found in the scene.");
            return null;
        }

        Node node = nodeGenerator.Nodes.Find(n => n.index == index);
        if (node == null)
        {
            Debug.LogError("Node not found for index: " + index);
        }

        return node;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 필요한 경우 동기화할 데이터를 추가합니다.
    }
}