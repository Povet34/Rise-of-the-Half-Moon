using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Search;
using UnityEngine;

public class RuleManager : Singleton<RuleManager>
{
    bool isAnimating = false;

    Queue<Action> animationQueue = new Queue<Action>();

    #region Calc Score

    private void Update()
    {
        if (!isAnimating && animationQueue.Count > 0)
        {
            animationQueue.Dequeue().Invoke();
        }
    }

    public void OnCardPlaced(Node node)
    {
        CheckAdjacentNodes(node);
        CheckFullMoon(node);
        CheckMoonCycle(node);
    }

    private void CheckAdjacentNodes(Node node)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.MoonPhaseData != null && adjacentNode.GetPhaseType() == node.GetPhaseType())
            {
                animationQueue.Enqueue(() =>
                {
                    bool isMine = node.OccupiedUser == Definitions.MY_INDEX;
                    AnimateNodes(new List<Node>() { node, adjacentNode }, isMine);
                    
                    if(isMine)
                        GameManager.Instance.UpdateMyScore(Definitions.SAME_PHASE_SCORE);
                    else
                        GameManager.Instance.UpdateOtherScore(Definitions.SAME_PHASE_SCORE);

                });
            }
        }
    }

    private void CheckFullMoon(Node node)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.MoonPhaseData != null && IsFullMoonCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                bool isMine = node.OccupiedUser == Definitions.MY_INDEX;
                AnimateNodes(new List<Node>() { node, adjacentNode }, isMine);

                if (isMine)
                    GameManager.Instance.UpdateMyScore(Definitions.SAME_PHASE_SCORE);
                else
                    GameManager.Instance.UpdateOtherScore(Definitions.SAME_PHASE_SCORE);
            }
        }
    }

    private bool IsFullMoonCombination(MoonPhaseData.PhaseType type1, MoonPhaseData.PhaseType type2)
    {
        // Implement the logic to check if the combination of type1 and type2 can form a full moon
        return (type1 == MoonPhaseData.PhaseType.NewMoon && type2 == MoonPhaseData.PhaseType.FullMoon) ||
               (type1 == MoonPhaseData.PhaseType.FullMoon && type2 == MoonPhaseData.PhaseType.NewMoon) ||
               (type1 == MoonPhaseData.PhaseType.WaningCrescent && type2 == MoonPhaseData.PhaseType.WaxingGibbous) ||
               (type1 == MoonPhaseData.PhaseType.WaxingGibbous && type2 == MoonPhaseData.PhaseType.WaningCrescent) ||
               (type1 == MoonPhaseData.PhaseType.ThirdQuarter && type2 == MoonPhaseData.PhaseType.FirstQuarter) ||
               (type1 == MoonPhaseData.PhaseType.FirstQuarter && type2 == MoonPhaseData.PhaseType.ThirdQuarter) ||
               (type1 == MoonPhaseData.PhaseType.WaningGibbous && type2 == MoonPhaseData.PhaseType.WaxingCrescent) ||
               (type1 == MoonPhaseData.PhaseType.WaxingCrescent && type2 == MoonPhaseData.PhaseType.WaningGibbous);
    }

    private void CheckMoonCycle(Node node)
    {
        ColorNodesByPhase(node);

        List <Node> cycleNodes = new List<Node>();
        if (IsMoonCycle(node, cycleNodes))
        {
            Debug.Log("The placed node forms a moon cycle.");
        }
    }

    private bool IsMoonCycle(Node node, List<Node> cycleNodes)
    {
        return false;
    }

    public void ColorNodesByPhase(Node startNode)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>(); // To track visited nodes and prevent revisiting

        queue.Enqueue(startNode);
        visited.Add(startNode); // Mark the start node as visited

        bool isDecreasing = false;
        bool isIncreasing = false;

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
            MoonPhaseData.PhaseType currentPhase = currentNode.GetPhaseType();

            foreach (Node neighbor in currentNode.GetAdjacentNodes())
            {
                if (!visited.Contains(neighbor))
                {
                    MoonPhaseData.PhaseType neighborPhase = neighbor.GetPhaseType();

                    if (!isDecreasing && !isIncreasing)
                    {
                        if (MoonPhaseData.GetPreviousPhaseType(currentPhase) == neighborPhase)
                        {
                            isDecreasing = true;
                        }
                        else if (MoonPhaseData.GetNextPhaseType(currentPhase) == neighborPhase)
                        {
                            isIncreasing = true;
                        }
                    }

                    if (isDecreasing && MoonPhaseData.GetPreviousPhaseType(currentPhase) == neighborPhase)
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor); // Mark as visited
                        //neighbor.ChangeColor(Color.blue); // Change color to indicate phase
                    }
                    else if (isIncreasing && MoonPhaseData.GetNextPhaseType(currentPhase) == neighborPhase)
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor); // Mark as visited
                        //neighbor.ChangeColor(Color.green); // Change color to indicate phase
                    }
                }
            }
        }
    }

    #endregion

    #region Animation

    public void AnimateNodes(List<Node> nodes, bool isMine)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (Node node in nodes)
        {
            Vector3 originalScale = node.transform.localScale;
            Vector3 targetScale = originalScale * 1.5f;

            sequence.AppendCallback(() => isAnimating = true);
            sequence.Append(node.transform.DOScale(targetScale, 0.5f));
            sequence.AppendCallback(() => node.EnableEmission(isMine ? Definitions.My_Occupied_Color : Definitions.Other_Occupied_Color));
            sequence.AppendInterval(0.5f);
            sequence.Append(node.transform.DOScale(originalScale, 0.5f));
            sequence.AppendCallback(() => isAnimating = false);
        }

        sequence.Play();
    }

    #endregion
}
