using System.Collections.Generic;

/// <summary>
/// NodeCycleHelper는 노드 간의 연결 관계를 기반으로 사이클(루프)을 찾는 데 사용되는 헬퍼 클래스입니다.
/// </summary>
public class NodeCycleHelper
{
    public int row; // 현재 노드의 행 갯수
    public int col; // 현재 노드의 열 갯수
    public List<Node> nodes = new List<Node>(); // 노드 리스트
    GameManager gameManager; // 게임 매니저 인스턴스

    /// <summary>
    /// NodeCycleHelper 생성자. 게임 매니저와 노드 리스트, 행/열 정보를 초기화합니다.
    /// </summary>
    public NodeCycleHelper(GameManager gameManager, List<Node> nodes, int row, int col)
    {
        this.gameManager = gameManager;
        this.row = row;
        this.col = col;
        this.nodes = nodes;
    }

    /// <summary>
    /// 특정 노드를 기준으로 사이클(루프)을 찾습니다.
    /// </summary>
    /// <param name="putNode">기준이 되는 노드</param>
    /// <returns>사이클을 이루는 노드들의 딕셔너리 (키: 고유 식별자, 값: 노드 리스트)</returns>
    public Dictionary<string, List<Node>> FindCycle(Node putNode)
    {
        Dictionary<string, List<Node>> resultDic = new Dictionary<string, List<Node>>();

        // 기준 노드와 인접한 노드 정보를 업데이트
        UpdateNearNodeInfo(putNode);

        List<List<Node>> nextNodeLists = new List<List<Node>>(); // 다음 노드 경로 리스트
        List<List<Node>> prevNodeLists = new List<List<Node>>(); // 이전 노드 경로 리스트

        // 다음 노드 경로를 설정
        List<Node> temp = new List<Node>();
        SetNextNodeList(ref nextNodeLists, ref temp, putNode);

        // 이전 노드 경로를 설정
        temp = new List<Node>();
        SetPrevNodeList(ref prevNodeLists, ref temp, putNode);

        // 이전 경로와 다음 경로를 조합하여 사이클을 찾음
        foreach (List<Node> prevs in prevNodeLists)
        {
            prevs.Reverse(); // 이전 경로를 역순으로 정렬

            foreach (List<Node> next in nextNodeLists)
            {
                List<Node> result = new List<Node>();

                // 다음 경로에서 이전 경로와 겹치는 부분을 제거
                List<Node> tmpNexts = new List<Node>(next);
                if (prevs.Count > 0)
                {
                    for (int i = 0; i < next.Count; i++)
                    {
                        if (next[i] == prevs[0])
                        {
                            tmpNexts = next.GetRange(0, i);
                            break;
                        }
                    }
                }

                // 사이클의 고유 키를 생성하고 결과 리스트에 추가
                string key = "";
                foreach (var pre in prevs)
                {
                    key += pre.index;
                    result.Add(pre);
                }

                key += putNode.index;
                result.Add(putNode);

                foreach (var nxt in tmpNexts)
                {
                    key += nxt.index;
                    result.Add(nxt);
                }

                // 사이클이 2개 이상의 노드로 이루어진 경우 결과에 추가
                if (result.Count > 2)
                    resultDic[key] = result;
            }
        }

        return resultDic;
    }

    /// <summary>
    /// 현재 노드에서 다음 노드 경로를 설정합니다.
    /// </summary>
    private void SetNextNodeList(ref List<List<Node>> nextNodeLists, ref List<Node> currentPathNode, Node node)
    {
        bool isFindNext = false;
        foreach (Node nextNode in node.nextNodes)
        {
            if (currentPathNode.Contains(nextNode))
                continue;
            isFindNext = true;
            List<Node> nodes = new List<Node>(currentPathNode);
            nodes.Add(nextNode);
            SetNextNodeList(ref nextNodeLists, ref nodes, nextNode);
        }
        if (!isFindNext)
        {
            nextNodeLists.Add(currentPathNode);
        }
    }

    /// <summary>
    /// 현재 노드에서 이전 노드 경로를 설정합니다.
    /// </summary>
    private void SetPrevNodeList(ref List<List<Node>> nextNodeLists, ref List<Node> currentPathNode, Node node)
    {
        bool isFindNext = false;
        foreach (Node nextNode in node.prevNodes)
        {
            if (currentPathNode.Contains(nextNode))
                continue;

            isFindNext = true;
            List<Node> nodes = new List<Node>(currentPathNode);
            nodes.Add(nextNode);
            SetPrevNodeList(ref nextNodeLists, ref nodes, nextNode);
        }
        if (!isFindNext)
        {
            nextNodeLists.Add(currentPathNode);
        }
    }

    /// <summary>
    /// 특정 노드와 인접한 노드들의 연결 정보를 업데이트합니다.
    /// </summary>
    private void UpdateNearNodeInfo(Node node)
    {
        var connectedNodes = node.GetAdjacentNodes();
        foreach (var connectedNode in connectedNodes)
        {
            if (IsNext(connectedNode, node))
            {
                node.prevNodes.Add(connectedNode);
                connectedNode.nextNodes.Add(node);
            }
            if (IsPrev(connectedNode, node))
            {
                node.nextNodes.Add(connectedNode);
                connectedNode.prevNodes.Add(node);
            }
        }
    }

    /// <summary>
    /// 두 노드가 "다음" 관계인지 확인합니다.
    /// </summary>
    private bool IsNext(Node node1, Node node2)
    {
        return PhaseData.GetNextPhaseType(node1.GetPhaseType(), gameManager.contentType) == node2.GetPhaseType();
    }

    /// <summary>
    /// 두 노드가 "이전" 관계인지 확인합니다.
    /// </summary>
    private bool IsPrev(Node node1, Node node2)
    {
        return PhaseData.GetPreviousPhaseType(node1.GetPhaseType(), gameManager.contentType) == node2.GetPhaseType();
    }
}
