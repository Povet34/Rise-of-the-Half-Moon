using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GridGeneratorWithEightDirections : MonoBehaviour
{
    public GameObject nodePrefab;  // Prefab for the nodes
    public GameObject edgePrefab;  // Prefab for the edges
    public int rows = 3;           // Number of rows
    public int cols = 4;           // Number of columns
    public float spacing = 2.0f;   // Spacing between nodes
    public int seed = 42;          // Random seed value

    private List<Node> nodes = new List<Node>();  // List to store all nodes
    private List<Edge> edges = new List<Edge>();  // List to store all edges

    // Lists to manage the instantiated GameObjects
    public List<GameObject> nodeObjects = new List<GameObject>();
    public List<GameObject> edgeObjects = new List<GameObject>();

    private Node selectedNode = null; // The currently selected node

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
        nodes.Clear();
        edges.Clear();

        Random.InitState(seed); // 랜덤 시드 초기화
        GenerateGrid();
        GenerateRandomConnections();
        EnsureAllNodesConnected();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(j * spacing, i * spacing, 0);
                GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity, transform);

                Node newNode = nodeObject.GetComponent<Node>();
                newNode.Init(position, nodeObject);

                nodes.Add(newNode);
                nodeObjects.Add(nodeObject);
            }
        }
    }

    // Generate random connections between the nodes
    void GenerateRandomConnections()
    {
        foreach (Node node in nodes)
        {
            List<Node> neighbors = GetNeighbors(node);

            foreach (Node neighbor in neighbors)
            {
                if (Random.value > 0.5f && !IsAlreadyConnected(node, neighbor))
                {
                    CreateEdge(node, neighbor);
                }
            }
        }
    }

    // Ensure all nodes are connected (using DFS)
    void EnsureAllNodesConnected()
    {
        HashSet<Node> visited = new HashSet<Node>();
        Stack<Node> stack = new Stack<Node>();

        visited.Add(nodes[0]);
        stack.Push(nodes[0]);

        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();

            foreach (Edge edge in currentNode.connectedEdges)
            {
                Node neighbor = edge.GetOtherNode(currentNode);
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }
            }
        }

        foreach (Node node in nodes)
        {
            if (!visited.Contains(node))
            {
                Node randomNode = nodes[Random.Range(0, nodes.Count)];
                CreateEdge(node, randomNode);
                visited.Add(node);
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
        // Initialize the edge, but do not instantiate it yet
        Vector3 midpoint = (nodeA.position + nodeB.position) / 2;
        GameObject edgeObject = Instantiate(edgePrefab, midpoint, Quaternion.identity, transform);

        Vector3 direction = nodeB.position - nodeA.position;
        edgeObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        edgeObject.transform.localScale = new Vector3(0.1f, direction.magnitude / 2, 0.1f);

        Edge newEdge = edgeObject.GetComponent<Edge>();
        newEdge.Init(nodeA, nodeB);

        // Add the edge to the node's connected edges list
        edges.Add(newEdge);
        edgeObjects.Add(edgeObject);

        nodeA.connectedEdges.Add(newEdge);
        nodeB.connectedEdges.Add(newEdge);
    }

    #region Detect node relationship

    // Node clicked, select and color based on distance in edges
    void DetectNode()
    {
        // Detect if a node is clicked
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            selectedNode = hitObject.GetComponent<Node>();

            // Change color of nodes based on distance (in edges) to selected node
            if (selectedNode != null)
            {
                ColorNodesByEdgeDistance(selectedNode);
            }
        }
    }

    // Perform BFS and color the nodes based on distance (edges)
    void ColorNodesByEdgeDistance(Node startNode)
    {
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, int> distances = new Dictionary<Node, int>(); // Dictionary to store distances from startNode
        HashSet<Node> visited = new HashSet<Node>(); // To track visited nodes and prevent revisiting

        // Initialize distances to infinity
        foreach (var node in nodes)
        {
            distances[node] = int.MaxValue;
        }

        // Initialize the start node
        distances[startNode] = 0;
        queue.Enqueue(startNode);
        visited.Add(startNode); // Mark the start node as visited

        // Change the color of the selected start node to red
        startNode.ChangeColor(Color.red);

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            int currentDistance = distances[currentNode];

            foreach (Edge edge in currentNode.connectedEdges)
            {
                Node neighbor = edge.GetOtherNode(currentNode);

                // If this node hasn't been visited yet
                if (!visited.Contains(neighbor))
                {
                    distances[neighbor] = currentDistance + 1;
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor); // Mark as visited

                    // Change the color of the neighbor node based on distance
                    neighbor.ChangeColorBasedOnEdgeDistance(distances[neighbor]);
                }
            }
        }
    }

    void ResetAllNodeColors()
    {
        foreach (Node node in nodes)
        {
            node.ResetColor();
        }
    }

    #endregion
}
