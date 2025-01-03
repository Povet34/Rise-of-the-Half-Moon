using Photon.Pun.Demo.PunBasics;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    const string BASE_MAP = "_BaseMap";
    const string EMISSION_COLOR = "_EmissionColor";

    GameManager gameManager;

    public struct PutData
    {
        public int occupiedUser;   //occupied user index
        public PhaseData moonPhaseData; //moon phase data
    }

    public struct InitData
    {
        public int index;
        public Vector3 position;
        public Transform myProfileTr;
        public Transform otherProfileTr;
    }

    public int index;
    public List<Edge> connectedEdges = new List<Edge>();
    public Vector3 position;
    public int occupiedUser;
    public PhaseData phaseData;

    private Renderer nodeRenderer;  // Renderer to change the color
    private MaterialPropertyBlock propertyBlock;

    [SerializeField] private RectTransform canvasRt;
    [SerializeField] private TextMeshProUGUI pointValueNotifier;
    [SerializeField] private ScoreStar scoreStarPrefab;

    public List<Node> nextNodes = new List<Node>();
    public List<Node> prevNodes = new List<Node>();

    private Transform myProfileTr;
    private Transform otherProfileTr;

    public void Init(InitData data)
    {
        index = data.index;
        position = data.position;
        myProfileTr = data.myProfileTr;
        otherProfileTr = data.otherProfileTr;

        occupiedUser = Definitions.EMPTY_NODE;

        gameManager = FindAnyObjectByType<GameManager>();
        nodeRenderer = GetComponent<Renderer>(); // Get the Renderer component
        propertyBlock = new MaterialPropertyBlock();

        pointValueNotifier.gameObject.SetActive(false);
    }

    public int GetPhaseType()
    {
        if (null != phaseData)
            return phaseData.phaseIndex;
        else
            return -100;
    }

    public void SetOccupiedUser(int userIndex)
    {
        occupiedUser = userIndex;
    }

    public List<Node> GetAdjacentNodes()
    {   
        List<Node> adjacentNodes = new List<Node>();
        foreach (Edge edge in connectedEdges)
        {
            Node adjacentNode = edge.GetOtherNode(this);
            if (adjacentNode != null)
            {
                adjacentNodes.Add(adjacentNode);
            }
        }
        return adjacentNodes;
    }

    public void PutCard(PutData data)
    {
        phaseData = data.moonPhaseData;
        occupiedUser = data.occupiedUser;

        nodeRenderer.GetPropertyBlock(propertyBlock); 
        propertyBlock.SetTexture(BASE_MAP, phaseData.GetTexture()); 
        nodeRenderer.SetPropertyBlock(propertyBlock);
    }

    public void EnableEmission(Color emissionColor)
    {
        if (nodeRenderer != null && nodeRenderer.material != null)
        {
            nodeRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(EMISSION_COLOR, emissionColor); // Change emission color
            nodeRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void EffectStar(bool isMine)
    {
        if (scoreStarPrefab != null)
        {
            var targetPos = isMine ? myProfileTr.position : otherProfileTr.position;
            ScoreStar scoreStar = Instantiate(scoreStarPrefab, transform.position, Quaternion.identity);
            scoreStar.DoEffect(isMine, targetPos);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}