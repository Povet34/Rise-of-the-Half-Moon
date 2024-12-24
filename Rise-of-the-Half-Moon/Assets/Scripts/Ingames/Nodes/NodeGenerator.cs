using System.Collections.Generic;
using System.Linq;
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

    public List<Node> Nodes => nodes;

    private List<NodePathInfo> pathInfos = new List<NodePathInfo>();
    private NodeCycleHelper nodeCycleHelper;

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
        // Check if either node already has 4 edges
        if (nodeA.connectedEdges.Count >= 3 || nodeB.connectedEdges.Count >= 3)
            return;

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

    private class NodePathInfo
    {
        public enum CreaseType
        {
            None,
            Increase,
            Decrease,
        }

        public Node EndNode { get; set; }
        public int Distance { get; set; }
        public List<Node> Path { get; set; }
        public CreaseType creaseType { get; set; }
    }

    private void FindContinuousCreases(Node startNode)
    {
        pathInfos.Clear();

        //Find Path
        FindPath(startNode, new List<Node> { startNode });

        //Marge Path
        MergePath();
    }

    private void FindPath(Node currentNode, List<Node> currentPath)
    {
        foreach (Node neighbor in GetConnectedNeighbors(currentNode))
        {
            if (currentPath.Contains(neighbor))
                continue;

            List<Node> path = new List<Node>(currentPath) { neighbor };
            NodePathInfo.CreaseType creaseType = NodePathInfo.CreaseType.None;

            int currentNodePhase = currentNode.GetPhaseType();
            int neighborPhase = neighbor.GetPhaseType();

            if (neighborPhase == PhaseData.GetNextPhaseType(currentNodePhase, gameManager.contentType))
            {
                creaseType = NodePathInfo.CreaseType.Increase;
            }
            else if (neighborPhase == PhaseData.GetPreviousPhaseType(currentNodePhase, gameManager.contentType))
            {
                creaseType = NodePathInfo.CreaseType.Decrease;
            }
            else
            {
                continue;
            }

            FindPathInDirection(neighbor, path, creaseType);
        }
    }

    private void FindPathInDirection(Node currentNode, List<Node> currentPath, NodePathInfo.CreaseType creaseType)
    {
        bool validPath = true;

        while (validPath)
        {
            validPath = false;
            foreach (Node nextNeighbor in GetConnectedNeighbors(currentNode))
            {
                if (currentPath.Contains(nextNeighbor))
                    continue;

                int currentNodePhase = currentNode.GetPhaseType();
                int nextNeighborPhase = nextNeighbor.GetPhaseType();

                if ((creaseType == NodePathInfo.CreaseType.Increase &&
                     nextNeighborPhase == PhaseData.GetNextPhaseType(currentNodePhase, gameManager.contentType)) ||
                    (creaseType == NodePathInfo.CreaseType.Decrease &&
                     nextNeighborPhase == PhaseData.GetPreviousPhaseType(currentNodePhase, gameManager.contentType)))
                {
                    currentPath.Add(nextNeighbor);
                    currentNode = nextNeighbor;
                    validPath = true;
                    break;
                }
            }

            if (!validPath)
            {
                NodePathInfo pathInfo = new NodePathInfo
                {
                    EndNode = currentNode,
                    Distance = currentPath.Count,
                    Path = currentPath,
                    creaseType = creaseType
                };
                pathInfos.Add(pathInfo);
            }
        }
    }

    private void MergePath()
    {
        List<NodePathInfo> newPathInfos = new List<NodePathInfo>();
        List<NodePathInfo> increasePaths = pathInfos.Where(p => p.creaseType == NodePathInfo.CreaseType.Increase).ToList();
        List<NodePathInfo> decreasePaths = pathInfos.Where(p => p.creaseType == NodePathInfo.CreaseType.Decrease).ToList();

        if (increasePaths.Count > 0 && decreasePaths.Count > 0)
        {
            foreach (var increasePath in increasePaths)
            {
                foreach (var decreasePath in decreasePaths)
                {
                    List<Node> mergedPath = new List<Node>(increasePath.Path);
                    mergedPath.AddRange(decreasePath.Path.Skip(1)); // Skip the first node to avoid duplication

                    // Remove duplicate nodes
                    mergedPath = mergedPath.Distinct().ToList();

                    NodePathInfo mergedPathInfo = new NodePathInfo
                    {
                        EndNode = decreasePath.EndNode,
                        Distance = mergedPath.Count,
                        Path = mergedPath,
                        creaseType = NodePathInfo.CreaseType.None // Merged path has no specific crease type
                    };
                    newPathInfos.Add(mergedPathInfo);
                }
            }
        }
        else
        {
            newPathInfos.AddRange(increasePaths);
            newPathInfos.AddRange(decreasePaths);
        }

        // 기존의 것들을 삭제
        pathInfos.Clear();
        pathInfos.AddRange(newPathInfos);
    }

    public List<List<Node>> GetSequentialPhaseNodes(Node putNode)
    {
        List<List<Node>> includedStartNodeCycles = nodeCycleHelper.FindCycle(putNode).Values.ToList();
        return includedStartNodeCycles;
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