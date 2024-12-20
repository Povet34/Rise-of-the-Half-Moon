using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    public GameObject nodePrefab;  // Prefab for the nodes
    public GameObject edgePrefab;  // Prefab for the edges
    public int rows = 3;           // Number of rows
    public int cols = 4;           // Number of columns
    public float spacing = 2.0f;   // Spacing between nodes

    private GameManager gameManager;
    private List<Node> nodes = new List<Node>();  // List to store all nodes
    private List<Edge> edges = new List<Edge>();  // List to store all edges

    // Lists to manage the instantiated GameObjects
    public List<GameObject> nodeObjects = new List<GameObject>();
    public List<GameObject> edgeObjects = new List<GameObject>();

    private Dictionary<int, List<Node>> cycles = new Dictionary<int, List<Node>>(); // Dictionary to store moon cycles

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

        foreach (GameObject nodeObject in nodeObjects)
        {
            Destroy(nodeObject);
        }

        foreach (GameObject edgeObject in edgeObjects)
        {
            Destroy(edgeObject);
        }

        nodes.Clear();
        edges.Clear();

        GenerateGrid();
        GenerateRandomConnections();
        EnsureAllNodesConnected();
    }

    void GenerateGrid()
    {
        int order = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(j * spacing, i * spacing, 0);
                GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.Euler(0, 0, 180), transform);

                Node newNode = nodeObject.GetComponent<Node>();
                newNode.Init(order++, position, nodeObject);

                nodes.Add(newNode);
                nodeObjects.Add(nodeObject);
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

    #region Node Cycle
    void GenerateCycles(Node startNode)
    {
        cycles.Clear();
        List<Node> visited = new List<Node>();
        List<Node> currentCycle = new List<Node>();
        PhaseData.ContentType ct = gameManager.contentType;

        void Traverse(Node currentNode, int direction, int cycleId)
        {
            visited.Add(currentNode);
            currentCycle.Add(currentNode);

            List<Node> neighbors = GetConnectedNeighbors(currentNode);
            foreach (Node neighbor in neighbors)
            {
                if (visited.Contains(neighbor)) continue;

                int currentPhase = currentNode.GetPhaseType();
                int neighborPhase = neighbor.GetPhaseType();
                bool isValid = false;

                if (direction == 0)
                {
                    isValid = neighborPhase == PhaseData.GetNextPhaseType(currentPhase, ct) || neighborPhase == PhaseData.GetPreviousPhaseType(currentPhase, ct);
                }
                else if (direction == 1)
                {
                    isValid = (neighborPhase == PhaseData.GetNextPhaseType(currentPhase, ct));
                }
                else if (direction == -1)
                {
                    isValid = (neighborPhase == PhaseData.GetPreviousPhaseType(currentPhase, ct));
                }

                if (isValid)
                {
                    int newDirection = (neighborPhase == PhaseData.GetNextPhaseType(currentPhase, ct)) ? 1 : -1;
                    Traverse(neighbor, newDirection, cycleId);
                }
            }
        }

        int cycleId = 0;
        Traverse(startNode, 0, cycleId);
        if (currentCycle.Count > 2)
        {
            _Sort();
            cycles[cycleId++] = new List<Node>(currentCycle);
        }

        _DebugCycles();


        void _Sort()
        {
            currentCycle.Sort((node1, node2) =>
            {
                int phase1 = node1.GetPhaseType();
                int phase2 = node2.GetPhaseType();
                if (PhaseData.GetNextPhaseType(phase1, ct) == phase2)
                {
                    return -1;
                }
                else if (PhaseData.GetPreviousPhaseType(phase1, ct) == phase2)
                {
                    return 1;
                }
                return 0;
            });
        }
        void _DebugCycles()
        {
            foreach (var cycle in cycles)
            {
                string nodes = "";
                string phases = "";
                foreach (var node in cycle.Value)
                {
                    nodes += $"{node.index} ";
                    phases += $"{node.GetPhaseType()} ";
                }
                Debug.Log($"Cycle {cycle.Key} [노드 자리 : {startNode.index} 타입 : {startNode.GetPhaseType()}]: [{nodes}] [{phases}]");
            }
        }
    }

    public List<List<Node>> GetSequentialPhaseNodes(Node putNode)
    {
        GenerateCycles(putNode);

        List<List<Node>> includedStartNodeCycles = new List<List<Node>>();

        foreach (var cycle in cycles)
        {
            if (cycle.Value.Contains(putNode))
            {
                List<Node> sortedCycle = new List<Node>(cycle.Value);
                sortedCycle.Sort((node1, node2) => node2.GetPhaseType().CompareTo(node1.GetPhaseType()));

                includedStartNodeCycles.Add(sortedCycle);
            }
        }

        return includedStartNodeCycles; // Return an empty list if no group contains the startNode
    }

    #endregion

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