using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NodeGenerator : MonoBehaviour
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

        seed = Random.Range(0, 10000);

        Random.InitState(seed); // 랜덤 시드 초기화
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
                GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.Euler(0,0,180), transform);

                Node newNode = nodeObject.GetComponent<Node>();
                newNode.Init(order++, position, nodeObject);

                nodes.Add(newNode);
                nodeObjects.Add(nodeObject);
            }
        }
    }

    // Generate random connections between the nodes
    void GenerateRandomConnections()
    {
        System.Random random = new System.Random(seed);
        System.Random random2 = new System.Random(seed + 100);

        foreach (Node node in nodes)
        {
            List<Node> neighbors = GetNeighbors(node);

            foreach (Node neighbor in neighbors)
            {
                float distance = Vector3.Distance(node.position, neighbor.position);
                if (distance <= (spacing + spacing * 0.5f) && !IsAlreadyConnected(node, neighbor))
                {
                    if (random.NextDouble() <= random2.NextDouble())
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

    #region Moon Cycle

    void GenerateCycles()
    {
        HashSet<Node> visited = new HashSet<Node>();
        int cycleId = 0;

        foreach (Node node in nodes)
        {
            if (!visited.Contains(node))
            {
                List<Node> phaseGroup = new List<Node>();
                Queue<Node> queue = new Queue<Node>();

                queue.Enqueue(node);
                visited.Add(node);

                while (queue.Count > 0)
                {
                    Node currentNode = queue.Dequeue();
                    phaseGroup.Add(currentNode);
                    int currentPhase = currentNode.GetPhaseType();

                    foreach (Node neighbor in currentNode.GetAdjacentNodes())
                    {
                        if (!visited.Contains(neighbor))
                        {
                            int neighborPhase = neighbor.GetPhaseType();

                            if (PhaseData.GetPreviousPhaseType(currentPhase, PVEGameManager.Instance.contentType) == neighborPhase ||
                                PhaseData.GetNextPhaseType(currentPhase, PVEGameManager.Instance.contentType) == neighborPhase)
                            {
                                queue.Enqueue(neighbor);
                                visited.Add(neighbor);
                            }
                        }
                    }
                }

                if (phaseGroup.Count > 2)
                {
                    cycles[cycleId++] = phaseGroup;
                }
            }
        }
    }

    public List<List<Node>> GetSequentialPhaseNodes(Node startNode)
    {
        GenerateCycles();

        List<List<Node>> includedStartNodeCycles = new List<List<Node>>();

        foreach (var cycle in cycles)
        {
            if (cycle.Value.Contains(startNode))
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
}
