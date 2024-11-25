using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 position;
    public List<Edge> connectedEdges = new List<Edge>();
    
    private Renderer renderer;  // Renderer to change the color
    private int occupiedUser;

    public void Init(Vector3 position, GameObject nodeObject)
    {
        this.position = position;
        renderer = nodeObject.GetComponent<Renderer>(); // Get the Renderer component
    }

    public void PutCard(MoonPhaseData data)
    {
        renderer.material.mainTexture = data.phaseTexture; // Set the texture to the phase texture
    }

    // This method changes the color of the node based on the number of connected edges
    public void ChangeColorBasedOnEdgeDistance(int edgeDistance)
    {
        if (!TestManager.Instance.isTest)
            return;

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
        if (!TestManager.Instance.isTest)
            return;

        ChangeColor(Color.white);
    }

    public void ChangeColor(Color color)
    {
        if (!TestManager.Instance.isTest)
            return;

        renderer.material.color = color;
    }
}