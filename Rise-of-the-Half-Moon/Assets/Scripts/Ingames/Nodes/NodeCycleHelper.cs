using System.Collections.Generic;
using UnityEngine;

public class NodeCycleHelper
{
    public int row;
    public int col;
    public List<Node> nodes = new List<Node>();
    GameManager gameManager;

    public NodeCycleHelper(GameManager gameManager, List<Node> nodes, int row, int col)
    {
        this.gameManager = gameManager;
        this.row = row;
        this.col = col;
        this.nodes = nodes;
    }

    public Dictionary<string, List<Node>> FindCycle(Node putNode)
    {
        Dictionary<string, List<Node>> resultDic = new Dictionary<string, List<Node>>();

        UpdateNearNodeInfo(putNode);

        List<List<Node>> nextNodeLists = new List<List<Node>>();
        List<List<Node>> prevNodeLists = new List<List<Node>>();
        List<Node> temp = new List<Node>();
        SetNextNodeList(ref nextNodeLists, ref temp, putNode);
        temp = new List<Node>();
        SetPrevNodeList(ref prevNodeLists, ref temp, putNode);
        foreach (List<Node> prevs in prevNodeLists)
        {
            prevs.Reverse();

            foreach (List<Node> next in nextNodeLists)
            {
                List<Node> result = new List<Node>();

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

                resultDic[key] = result;
            }
        }

        return resultDic;
    }

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

    public void UpdateNearNodeInfo(Node node)
    {
        Node upNode = GetUpNode(node);
        if (upNode != null)
        {
            if (IsNext(upNode, node))
            {
                node.prevNodes.Add(upNode);
                upNode.nextNodes.Add(node);
            }
            if (IsPrev(upNode, node))
            {
                node.nextNodes.Add(upNode);
                upNode.prevNodes.Add(node);
            }
        }
        Node downNode = GetDownIndex(node);
        if (downNode != null)
        {
            if (IsNext(downNode, node))
            {
                node.prevNodes.Add(downNode);
                downNode.nextNodes.Add(node);
            }
            if (IsPrev(downNode, node))
            {
                node.nextNodes.Add(downNode);
                downNode.prevNodes.Add(node);
            }
        }
        Node leftNode = GetLeftIndex(node);
        if (leftNode != null)
        {
            if (IsNext(leftNode, node))
            {
                node.prevNodes.Add(leftNode);
                leftNode.nextNodes.Add(node);
            }
            if (IsPrev(leftNode, node))
            {
                node.nextNodes.Add(leftNode);
                leftNode.prevNodes.Add(node);
            }
        }
        Node rightNode = GetRightIndex(node);
        if (rightNode != null)
        {
            if (IsNext(rightNode, node))
            {
                node.prevNodes.Add(rightNode);
                rightNode.nextNodes.Add(node);
            }
            if (IsPrev(rightNode, node))
            {
                node.nextNodes.Add(rightNode);
                rightNode.prevNodes.Add(node);
            }
        }
    }
    private bool IsNext(Node node1, Node node2)
    {
        return PhaseData.GetNextPhaseType(node1.GetPhaseType(), gameManager.contentType) == node2.GetPhaseType();
    }

    private bool IsPrev(Node node1, Node node2)
    {
        return PhaseData.GetPreviousPhaseType(node1.GetPhaseType(), gameManager.contentType) == node2.GetPhaseType();
    }

    public Node GetUpNode(Node node)
    {
        if (node.index < row)
            return null;

        var target = nodes[node.index - col];

        if (!node.IsConnected(target))
            return null;

        return target;
    }

    public Node GetDownIndex(Node node)
    {
        if (node.index >= row * col)
            return null;

        var target = nodes[node.index + col];

        if (!node.IsConnected(target))
            return null;

        return target;
    }

    public Node GetLeftIndex(Node node)
    {
        if (node.index % col == 0)
            return null;

        var target = nodes[node.index - 1];

        if (!node.IsConnected(target))
            return null;

        return target;
    }

    public Node GetRightIndex(Node node)
    {
        if (node.index % col == 9)
            return null;

        var target = nodes[node.index + 1];

        if (!node.IsConnected(target))
            return null;

        return target;
    }
}
