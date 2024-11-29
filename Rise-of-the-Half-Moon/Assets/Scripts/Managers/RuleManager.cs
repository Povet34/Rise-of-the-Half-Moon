using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : Singleton<RuleManager>
{
    public bool isAnimating = false;

    NodeGenerator nodeGenerator;
    Queue<Action> animationQueue = new Queue<Action>();

    #region Calc Score

    private void Awake()
    {
        nodeGenerator = FindObjectOfType<NodeGenerator>();
    }

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
                AddAnimateQueue(node, new List<Node>() { node, adjacentNode });
            }
        }
    }

    private void CheckFullMoon(Node node)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.MoonPhaseData != null && IsFullMoonCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                AddAnimateQueue(node, new List<Node>() { node, adjacentNode });
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
        List<List<Node>> cycles = nodeGenerator.GetSequentialPhaseNodes(node);

        foreach(var cycle in cycles)
            AddAnimateQueue(node, cycle);
    }

    #endregion

    #region Infer Score

    public int PredictScoreForCard(Node node, Card card)
    {
        int totalScore = 0;

        // Check adjacent nodes for same phase type
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.MoonPhaseData != null && adjacentNode.GetPhaseType() == card.moonPhaseData.phaseType)
            {
                totalScore += Definitions.SAME_PHASE_SCORE;
            }
        }

        // Check adjacent nodes for full moon combination
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.MoonPhaseData != null && IsFullMoonCombination(card.moonPhaseData.phaseType, adjacentNode.GetPhaseType()))
            {
                totalScore += Definitions.FULL_MOON_SCORE;
            }
        }

        // Check for moon cycle
        List<List<Node>> moonCycleNodes = nodeGenerator.GetSequentialPhaseNodes(node);
        if (moonCycleNodes.Count > 0)
        {
            foreach (var cycle in moonCycleNodes)
                totalScore += Definitions.PHASE_CYCLE_SCORE * cycle.Count;
        }

        return totalScore;
    }

    #endregion

    #region Animation

    private void AddAnimateQueue(Node fristNode, List<Node> nodes)
    {
        bool isMine = fristNode.OccupiedUser == Definitions.MY_INDEX;
        AnimateNodes(nodes, isMine);

        if (isMine)
            GameManager.Instance.UpdateMyScore(Definitions.FULL_MOON_SCORE);
        else
            GameManager.Instance.UpdateOtherScore(Definitions.FULL_MOON_SCORE);

    }

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
