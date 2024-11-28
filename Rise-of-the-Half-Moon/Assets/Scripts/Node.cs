using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : MonoBehaviour
{
    public class PutData
    {
        public int occupiedUser;   //occupied user index
        public MoonPhaseData moonPhaseData; //moon phase data
    }

    private Renderer nodeRenderer;  // Renderer to change the color
    public Vector3 position;
    public List<Edge> connectedEdges = new List<Edge>();

    private int occupiedUser;   //occupied user index
    private MoonPhaseData moonPhaseData;

    public int OccupiedUser => occupiedUser;
    public MoonPhaseData MoonPhaseData => moonPhaseData;

    public void Init(Vector3 position, GameObject nodeObject)
    {
        this.position = position;
        nodeRenderer = nodeObject.GetComponent<Renderer>(); // Get the Renderer component
    }

    public MoonPhaseData.PhaseType GetPhaseType()
    {
        if (null != moonPhaseData)
            return moonPhaseData.phaseType;
        else
            return MoonPhaseData.PhaseType.None;
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
            moonPhaseData = data.moonPhaseData;
            occupiedUser = data.occupiedUser;

            nodeRenderer.material.mainTexture = moonPhaseData.phaseTexture; // Set the texture to the phase texture

            int score = RuleManager.Instance.OnCardPlaced(this);

            ////Place Animation
            //transform.DOShakePosition(0.5f, 0.5f, 10, 90, false, true);
            //transform.DOShakeScale(0.5f, 0.5f, 10, 90, false);

            if (data.occupiedUser == Definitions.MY_INDEX)
            {
                GameManager.Instance.UpdateMyScore(score);
            }
            else
            {
                GameManager.Instance.UpdateOtherScore(score);
            }
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
            nodeRenderer.material.EnableKeyword("_EMISSION");
            nodeRenderer.material.SetColor("_EmissionColor", emissionColor);
        }
    }
}