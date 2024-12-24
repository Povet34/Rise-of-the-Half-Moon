using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private TextMeshProUGUI pointValueNotifier;

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

    // This method changes the color of the node based on the number of connected edges
    public void ChangeColorBasedOnEdgeDistance(int edgeDistance)
    {
        switch (edgeDistance)
        {
            case 0:
                ChangeColor(Color.red);
                break;
            case 1:
                ChangeColor(Color.yellow);  // Edge 1 (yellow)
                break;
            case 2:
                ChangeColor(Color.green);   // Edge 2 (green)
                break;
            case 3:
                ChangeColor(Color.blue);    // Edge 3 (blue)
                break;
            case 4:
                ChangeColor(Color.magenta); // Edge 4 (purple)
                break;
            default:
                ChangeColor(Color.white);   // Default color (white)
                break;
        }
    }

    // Reset color to white
    public void ResetColor()
    {
        ChangeColor(Color.white);
    }

    public void ChangeColor(Color color)
    {
        nodeRenderer.material.color = color;
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

    public void UpdatePointValue(int value)
    {
        //if (TestManager.Instance.isTest)
        //{
        //    pointValueNotifier.text = value.ToString();
        //}
    }

    public bool IsConnected(Node target)
    {
        foreach (Edge edge in connectedEdges)
        {
            if (edge.nodeA == target || edge.nodeB == target)
            {
                return true;
            }
        }
        return false;
    }
}