using UnityEngine;
using System.Collections.Generic;
using System;

public class Node : MonoBehaviour
{
    public Vector3 position;
    public List<Edge> connectedEdges = new List<Edge>();
    private Renderer nodeRenderer;  // Renderer to change the color

    public void Init(Vector3 position, GameObject nodeObject)
    {
        this.position = position;
        nodeRenderer = nodeObject.GetComponent<Renderer>(); // Get the Renderer component
    }

    // This method changes the color of the node based on the number of connected edges
    public void ChangeColorBasedOnEdgeDistance(int edgeDistance)
    {
        switch (edgeDistance)
        {
            case 1:
                nodeRenderer.material.color = Color.yellow;  // Edge 1 (yellow)
                break;
            case 2:
                nodeRenderer.material.color = Color.green;   // Edge 2 (green)
                break;
            case 3:
                nodeRenderer.material.color = Color.blue;    // Edge 3 (blue)
                break;
            case 4:
                nodeRenderer.material.color = Color.magenta; // Edge 4 (purple)
                break;
            default:
                nodeRenderer.material.color = Color.white;   // Default color (white)
                break;
        }
    }

    // Reset color to white
    public void ResetColor()
    {
        nodeRenderer.material.color = Color.white;  // Default color is white
    }
}