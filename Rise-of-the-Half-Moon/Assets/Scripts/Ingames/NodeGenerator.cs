using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    // startNode로부터 모든 노드까지의 거리를 구한다.
    private Dictionary<Node, NodePathInfo> GetReachableNodes(Node startNode)
    {
        Dictionary<Node, NodePathInfo> reachableNodes = new Dictionary<Node, NodePathInfo>();
        Queue<(Node node, int distance, List<Node> path)> queue = new Queue<(Node node, int distance, List<Node> path)>();

        queue.Enqueue((startNode, 0, new List<Node> { startNode }));
        reachableNodes[startNode] = new NodePathInfo { EndNode = startNode, Distance = 0, Path = new List<Node> { startNode } };

        while (queue.Count > 0)
        {
            var (currentNode, currentDistance, currentPath) = queue.Dequeue();

            foreach (Node neighbor in GetConnectedNeighbors(currentNode))
            {
                if (!reachableNodes.ContainsKey(neighbor))
                {
                    int newDistance = currentDistance + 1;
                    List<Node> newPath = new List<Node>(currentPath) { neighbor };
                    reachableNodes[neighbor] = new NodePathInfo { EndNode = neighbor, Distance = newDistance, Path = newPath };
                    queue.Enqueue((neighbor, newDistance, newPath));
                }
            }
        }

        return reachableNodes;
    }

    private List<NodePathInfo> GetNodesByEdgeCount(Node startNode)
    {
        List<NodePathInfo> nodePathInfos = new List<NodePathInfo>();

        Dictionary<Node, NodePathInfo> reachableNodes = GetReachableNodes(startNode);

        foreach (var kvp in reachableNodes)
        {
            Node node = kvp.Key;
            NodePathInfo pathInfo = kvp.Value;
            int distance = pathInfo.Distance;

            // 경로에 있는 모든 노드가 유효한지 확인
            bool isValidPath = true;
            foreach (Node pathNode in pathInfo.Path)
            {
                if (pathNode == null || pathNode.phaseData == null)
                {
                    isValidPath = false;
                    break;
                }
            }

            if (isValidPath && distance >= 1)
            {
                pathInfo.creaseType = IsPathIncreasingOrDecreasing(pathInfo);
                nodePathInfos.Add(pathInfo);
            }
        }


        return nodePathInfos;
    }

    //start부터 end까지 증가/감소 연결성인지 확인
    private NodePathInfo.CreaseType IsPathIncreasingOrDecreasing(NodePathInfo pathInfo)
    {
        if (pathInfo.Path.Count < 2)
        {
            return NodePathInfo.CreaseType.None; // 경로에 노드가 1개 이하인 경우, None으로 간주
        }

        bool isIncreasing = true;
        bool isDecreasing = true;

        for (int i = 1; i < pathInfo.Path.Count; i++)
        {
            int previousPhase = pathInfo.Path[i - 1].phaseData.phaseIndex;
            int currentPhase = pathInfo.Path[i].phaseData.phaseIndex;
            PhaseData.ContentType contentType = pathInfo.Path[i].phaseData.contentType;

            if (PhaseData.GetNextPhaseType(previousPhase, contentType) != currentPhase)
            {
                isIncreasing = false;
            }

            if (PhaseData.GetPreviousPhaseType(previousPhase, contentType) != currentPhase)
            {
                isDecreasing = false;
            }
        }

        if (isIncreasing)
        {
            return NodePathInfo.CreaseType.Increase;
        }
        else if (isDecreasing)
        {
            return NodePathInfo.CreaseType.Decrease;
        }
        else
        {
            return NodePathInfo.CreaseType.None;
        }
    }

    //NodePathInfo.CreaseType이 None이면 제거
    private void RemoveNoneCreaseType(List<NodePathInfo> nodePaths)
    {
        nodePaths.RemoveAll(pathInfo => pathInfo.creaseType == NodePathInfo.CreaseType.None);
    }

    /// <summary>
    /// 최소량보다 적으면 제거한다
    /// </summary>
    /// <param name="minimum"></param>
    private void RemoveLessthanCount(int minimum, List<NodePathInfo> nodePaths)
    {
        nodePaths.RemoveAll(pathInfo => pathInfo.Path.Count < minimum);
    }

    //상위 경로가 하위 경로를 포함하는 경우 최상위 경로만 남기고 하위 경로를 모두 제거
    private void RemoveDuplicationNodePath(List<NodePathInfo> nodePaths)
    {
        for (int i = 0; i < nodePaths.Count; i++)
        {
            for (int j = nodePaths.Count - 1; j > i; j--)
            {
                if (IsPathContained(nodePaths[i], nodePaths[j]))
                {
                    nodePaths.RemoveAt(j);
                }
                else if (IsPathContained(nodePaths[j], nodePaths[i]))
                {
                    nodePaths.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }
    }

    // 경로가 다른 경로를 포함하는지 확인하는 메서드
    private bool IsPathContained(NodePathInfo pathInfo1, NodePathInfo pathInfo2)
    {
        return !pathInfo2.Path.Except(pathInfo1.Path).Any();
    }

    //증가 path와 감소 path가 있고, 0번이 같다면 증가 path와 감소 path가 합쳐진 새로운 NodePath를 만들어서 추가한다.
    private void MargeNodePath(List<NodePathInfo> nodePaths)
    {
        for (int i = 0; i < nodePaths.Count; i++)
        {
            for (int j = i + 1; j < nodePaths.Count; j++)
            {
                if (nodePaths[i].Path[0] == nodePaths[j].Path[0] &&
                    nodePaths[i].creaseType == NodePathInfo.CreaseType.Increase &&
                    nodePaths[j].creaseType == NodePathInfo.CreaseType.Decrease)
                {
                    List<Node> mergedPath = new List<Node>(nodePaths[i].Path);
                    mergedPath.AddRange(nodePaths[j].Path.Skip(1));

                    NodePathInfo mergedPathInfo = new NodePathInfo
                    {
                        EndNode = nodePaths[j].EndNode,
                        Distance = nodePaths[i].Distance + nodePaths[j].Distance - 1,
                        Path = mergedPath,
                        creaseType = NodePathInfo.CreaseType.None
                    };

                    nodePaths.Add(mergedPathInfo);
                    break;
                }
            }
        }
    }


    private void GenerateCycles(Node startNode)
    {
        pathInfos = GetNodesByEdgeCount(startNode);

        RemoveNoneCreaseType(pathInfos);
        MargeNodePath(pathInfos);
        RemoveLessthanCount(3, pathInfos);

        RemoveDuplicationNodePath(pathInfos);
    }

    public List<List<Node>> GetSequentialPhaseNodes(Node putNode)
    {
        GenerateCycles(putNode);

        List<List<Node>> includedStartNodeCycles = new List<List<Node>>();

        foreach (var cycle in pathInfos)
        {
            List<Node> sortedCycle = new List<Node>(cycle.Path);
            sortedCycle.Sort((node1, node2) => node2.GetPhaseType().CompareTo(node1.GetPhaseType()));

            includedStartNodeCycles.Add(sortedCycle);
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