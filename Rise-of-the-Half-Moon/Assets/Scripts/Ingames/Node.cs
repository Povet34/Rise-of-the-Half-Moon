using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public class PutData
    {
        public int occupiedUser;   //occupied user index
        public PhaseData moonPhaseData; //moon phase data
    }

    public int index;
    public List<Edge> connectedEdges = new List<Edge>();
    public Vector3 position;

    private Renderer nodeRenderer;  // Renderer to change the color
    private MaterialPropertyBlock propertyBlock;
    

    public int occupiedUser;

    public PhaseData phaseData;

    [SerializeField] private RectTransform canvasRt;
    [SerializeField] private TextMeshProUGUI pointValueNotifier;
    [SerializeField] private ScoreStar scoreStarPrefab;

    public List<Node> nextNodes = new List<Node>();
    public List<Node> prevNodes = new List<Node>();

    public void Init(int index, Vector3 position, GameObject nodeObject)
    {
        this.index = index;
        this.position = position;
        occupiedUser = Definitions.EMPTY_NODE;
        
        nodeRenderer = nodeObject.GetComponent<Renderer>(); // Get the Renderer component
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
        if (null != data)
        {
            phaseData = data.moonPhaseData;
            occupiedUser = data.occupiedUser;

            nodeRenderer.GetPropertyBlock(propertyBlock); // Retrieve current properties
            propertyBlock.SetTexture("_BaseMap", phaseData.GetTexture()); // Change texture
            nodeRenderer.SetPropertyBlock(propertyBlock); // Apply changes
        }
    }

    public void EnableEmission(Color emissionColor)
    {
        if (nodeRenderer != null && nodeRenderer.material != null)
        {
            nodeRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_EmissionColor", emissionColor); // Change emission color
            nodeRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void EffectStar(RectTransform attractor)
    {
        if (scoreStarPrefab != null)
        {
            //Instantiate
            //DoEffect
        }
    }
}