using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    [SerializeField] Node nodePrefab;        // Prefab for the nodes
    [SerializeField] Edge edgePrefab;        // Prefab for the edges
    [SerializeField] int rows = 3;           // Number of rows
    [SerializeField] int cols = 4;           // Number of columns
    [SerializeField] float spacing = 2.0f;   // Spacing between nodes

    GameManager gameManager;
    List<Node> nodes = new List<Node>();  // List to store all nodes
    List<Edge> edges = new List<Edge>();  // List to store all edges
    NodeCycleHelper nodeCycleHelper;

    public List<Node> Nodes => nodes;
    readonly Vector3[] validDirections = new Vector3[]
    {
        Vector3.right,
        Vector3.left,
        Vector3.up,
        Vector3.down
    };

    public void Create()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        foreach (var node in nodes)
            node.Destroy();

        foreach (var edge in edges)
            edge.Destroy();

        nodes.Clear();
        edges.Clear();

        GenerateGrid();
        GenerateRandomConnections();
        EnsureAllNodesConnected();

        nodeCycleHelper = new NodeCycleHelper(gameManager, Nodes, rows, cols);
    }

    void GenerateGrid()
    {
        int order = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(j * spacing, i * spacing, 0);
                Node node = Instantiate(nodePrefab, position, Quaternion.Euler(0, 0, 180), transform);
                node.Init(order++, position);
                nodes.Add(node);
            }
        }
    }

    void GenerateRandomConnections()
    {
        foreach (Node node in nodes)
        {
            List<Node> neighbors = GetNeighbors(node);

            // Ensure each node has at least one connection
            if (node.connectedEdges.Count == 0 && neighbors.Count > 0)
            {
                Node randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                CreateEdge(node, randomNeighbor);
            }

            // Create additional random connections
            foreach (Node neighbor in neighbors)
            {
                if (!IsAlreadyConnected(node, neighbor))
                {
                    if (Random.value <= 0.5f) // Adjust the probability as needed
                    {
                        CreateEdge(node, neighbor);
                    }
                }
            }
        }
    }

    void EnsureAllNodesConnected()
    {
        foreach (Node node in nodes)
        {
            if (node.connectedEdges.Count == 0)
            {
                List<Node> neighbors = GetNeighbors(node);
                if (neighbors.Count > 0)
                {
                    Node randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                    CreateEdge(node, randomNeighbor);
                }
            }
        }
    }

    // Get the neighboring nodes (valid 8-direction check)
    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        Vector3 position = node.position;

        foreach (Node potentialNeighbor in nodes)
        {
            if (potentialNeighbor == node) continue;

            Vector3 difference = potentialNeighbor.position - position;

            if (IsValidDirection(difference) && difference.magnitude <= spacing * Mathf.Sqrt(2))
            {
                neighbors.Add(potentialNeighbor);
            }
        }

        return neighbors;
    }

    // Check if the direction is valid (8 directions)
    bool IsValidDirection(Vector3 difference)
    {
        difference.Normalize();
        difference = new Vector3(Mathf.Round(difference.x), Mathf.Round(difference.y), 0);

        foreach (Vector3 dir in validDirections)
        {
            if (difference == dir)
                return true;
        }

        return false;
    }

    // Check if two nodes are already connected
    bool IsAlreadyConnected(Node nodeA, Node nodeB)
    {
        foreach (Edge edge in edges)
        {
            if ((edge.nodeA == nodeA && edge.nodeB == nodeB) ||
                (edge.nodeA == nodeB && edge.nodeB == nodeA))
            {
                return true;
            }
        }
        return false;
    }

    // Create an edge between two nodes (do not instantiate Edge here)
    void CreateEdge(Node nodeA, Node nodeB)
    {
        // Check if either node already has 4 edges
        if (nodeA.connectedEdges.Count >= 3 || nodeB.connectedEdges.Count >= 3)
            return;

        // Initialize the edge, but do not instantiate it yet
        Vector3 midpoint = (nodeA.position + nodeB.position) / 2;
        Edge edge = Instantiate(edgePrefab, midpoint, Quaternion.identity, transform);

        Vector3 direction = nodeB.position - nodeA.position;
        edge.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        edge.transform.localScale = new Vector3(0.1f, direction.magnitude / 2, 0.1f);
        edge.Init(nodeA, nodeB);

        // Add the edge to the node's connected edges list
        edges.Add(edge);

        nodeA.connectedEdges.Add(edge);
        nodeB.connectedEdges.Add(edge);
    }

    List<Node> GetConnectedNeighbors(Node node)
    {
        List<Node> connectedNeighbors = new List<Node>();
        foreach (Edge edge in node.connectedEdges)
        {
            Node neighbor = edge.GetOtherNode(node);
            connectedNeighbors.Add(neighbor);
        }
        return connectedNeighbors;
    }

    public List<List<Node>> GetSequentialPhaseNodes(Node putNode)
    {
        List<List<Node>> includedStartNodeCycles = nodeCycleHelper.FindCycle(putNode).Values.ToList();
        return includedStartNodeCycles;
    }


    public List<Node> FindEmptyOccupidNodes()
    {
        List<Node> emptyNodes = new List<Node>();
        foreach (Node node in nodes)
        {
            if (node.occupiedUser == Definitions.EMPTY_NODE)
            {
                emptyNodes.Add(node);
            }
        }

        return emptyNodes;
    }

    public bool IsEndGame()
    {
        return FindEmptyOccupidNodes().Count == 0;
    }

    public Bounds GetNodeBounds()
    {
        if (nodes.Count == 0)
        {
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        Vector3 min = nodes[0].position;
        Vector3 max = nodes[0].position;

        foreach (Node node in nodes)
        {
            min = Vector3.Min(min, node.position);
            max = Vector3.Max(max, node.position);
        }

        Vector3 center = (min + max) / 2;
        Vector3 size = max - min;

        return new Bounds(center, size);
    }
}