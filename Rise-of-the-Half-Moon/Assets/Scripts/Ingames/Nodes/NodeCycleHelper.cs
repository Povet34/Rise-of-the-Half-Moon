using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCycleHelper
{
    public int row;
    public int col;
    public List<Node> nodes = new List<Node>();
    GameManager gameManager;

    public void Init(GameManager gameManager, List<Node> nodes, int row, int col)
    {
        this.gameManager = gameManager;
        this.row = row;
        this.col = col;
        this.nodes = nodes; 
    }

    public void CheckCombo(Node putNode)
    {
        UpdateNearNodeInfo(putNode);
        Stack<Node> stack = new Stack<Node>();
        stack.Push(putNode);
        List<List<Node>> nextNodeLists = new List<List<Node>>();
        List<List<Node>> prevNodeLists = new List<List<Node>>();
        List<Node> temp = new List<Node>();
        SetNextNodeList(ref nextNodeLists, ref temp, putNode);
        temp = new List<Node>();
        SetPrevNodeList(ref prevNodeLists, ref temp, putNode);
        foreach (List<Node> prev in prevNodeLists)
        {
            prev.Reverse();
            foreach (List<Node> next in nextNodeLists)
            {
                List<Node> tmpNext = new List<Node>(next);
                if (prev.Count > 0)
                {
                    for (int i = 0; i < next.Count; i++)
                    {
                        if (next[i] == prev[0])
                        {
                            tmpNext = next.GetRange(0, i);
                            break;
                        }
                    }
                }
                string str = string.Empty;
                foreach (var tt in prev)
                {
                    str += tt.GetPhaseType().ToString();
                    str += $"({tt.index})";
                    str += "-";
                }
                str += putNode.GetPhaseType().ToString();
                str += $"({putNode.index})";
                str += "-";
                foreach (var tt in tmpNext)
                {
                    str += tt.GetPhaseType().ToString();
                    str += $"({tt.index})";
                    str += "-";
                }
                Debug.Log(str);
            }
        }
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
        Node upNode = GetUpNode(node.index);
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
        Node downNode = GetDownIndex(node.index);
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
        Node leftNode = GetLeftIndex(node.index);
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
        Node rightNode = GetRightIndex(node.index);
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
    public Node GetUpNode(int index)
    {
        if (index < row)
            return null;
        return nodes[index - col];
    }
    public Node GetDownIndex(int index)
    {
        if (index >= row * col)
            return null;
        return nodes[index + col]; 
    }
    public Node GetLeftIndex(int index)
    {
        if (index % col == 0)
            return null;
        return nodes[index - 1];
    }
    public Node GetRightIndex(int index)
    {
        if (index % col == 9)
            return null;
        return nodes[index + 1];
    }
}
