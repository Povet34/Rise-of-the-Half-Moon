using UnityEngine;

public class Edge : MonoBehaviour
{
    public Node nodeA;      // The first node of the edge
    public Node nodeB;      // The second node of the edge

    // Init method to initialize an edge between two nodes
    public void Init(Node nodeA, Node nodeB)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
    }

    // Method to get the other node in case of traversal
    public Node GetOtherNode(Node currentNode)
    {
        if (currentNode == nodeA)
        {
            return nodeB;
        }
        else if (currentNode == nodeB)
        {
            return nodeA;
        }
        return null;
    }
}