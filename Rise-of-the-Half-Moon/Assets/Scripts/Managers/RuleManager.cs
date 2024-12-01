using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuleManager : Singleton<RuleManager>
{
    private bool isAnimating = false;

    NodeGenerator nodeGenerator;
    Queue<Action> animationQueue = new Queue<Action>();

    #region Calc Score

    private void Awake()
    {
        nodeGenerator = FindObjectOfType<NodeGenerator>();
        StartCoroutine(DelayAnimation());
    }


    public bool IsRemainScoreSettlement()
    {
        return animationQueue.Count > 0 || isAnimating;
    }

    public void SetIsAnimation(bool isOn)
    {
        isAnimating = isOn;
    }

    public void SettlementOccupiedNodes(Action settlementEndCallback)
    {
        List<Node> myOccupiedNodes = new List<Node>();
        List<Node> otherOccupiedNodes = new List<Node>();

        foreach (Node node in nodeGenerator.Nodes)
        {
            if (node.occupiedUser == Definitions.MY_INDEX)
            {
                myOccupiedNodes.Add(node);
            }
            else
            {
                otherOccupiedNodes.Add(node);
            }
        }

        AddAnimateQueue(true, myOccupiedNodes, Definitions.SETTLEMENT_SCORE, settlementEndCallback);
        AddAnimateQueue(false, otherOccupiedNodes, Definitions.SETTLEMENT_SCORE, settlementEndCallback);
    }


    public void OnCardPlaced(Node node, bool isMine)
    {
        CheckAdjacentNodes(node, isMine);
        CheckFullMoon(node, isMine);
        CheckMoonCycle(node, isMine);
    }

    private void CheckAdjacentNodes(Node node, bool isMine)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.moonPhaseData != null && adjacentNode.GetPhaseType() == node.GetPhaseType())
            {
                AddAnimateQueue(isMine, new List<Node>() { node, adjacentNode }, Definitions.SAME_PHASE_SCORE);
            }
        }
    }

    private void CheckFullMoon(Node node, bool isMine)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.moonPhaseData != null && IsFullMoonCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                AddAnimateQueue(isMine, new List<Node>() { node, adjacentNode }, Definitions.FULL_MOON_SCORE);
            }
        }
    }

    private void CheckMoonCycle(Node node, bool isMine)
    {
        List<List<Node>> cycles = nodeGenerator.GetSequentialPhaseNodes(node);

        foreach (var cycle in cycles)
            AddAnimateQueue(isMine, cycle, Definitions.PHASE_CYCLE_SCORE);
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

    #endregion

    #region Infer Score

    public int PredictScoreForCard(Node node, Card card)
    {
        int totalScore = 0;

        // Check adjacent nodes for same phase type
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.moonPhaseData != null && adjacentNode.GetPhaseType() == card.moonPhaseData.phaseType)
            {
                totalScore += Definitions.SAME_PHASE_SCORE;
            }
        }

        // Check adjacent nodes for full moon combination
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.moonPhaseData != null && IsFullMoonCombination(card.moonPhaseData.phaseType, adjacentNode.GetPhaseType()))
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

    private IEnumerator DelayAnimation()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.25f);

            if (!isAnimating && animationQueue.Count > 0)
            {
                animationQueue.Dequeue().Invoke();
            }
        }
    }

    private void AddAnimateQueue(bool isMine, List<Node> nodes, int score, Action endCallback = null)
    {
        animationQueue.Enqueue(() => 
        {
            AnimateNodes(nodes, isMine, endCallback);

            if (isMine)
                GameManager.Instance.UpdateMyScore(score);
            else
                GameManager.Instance.UpdateOtherScore(score);
        });
    }

    public void AnimateNodes(List<Node> nodes, bool isMine, Action endCallback)
    {
        Sequence sequence = DOTween.Sequence();
        SetIsAnimation(true);

        foreach (Node node in nodes)
        {
            Vector3 originalScale = node.transform.localScale;
            Vector3 targetScale = originalScale * 1.5f;

            sequence.Append(node.transform.DOScale(targetScale, 0.5f));
            sequence.AppendCallback(() => node.EnableEmission(isMine ? Definitions.My_Occupied_Color : Definitions.Other_Occupied_Color));
            sequence.AppendCallback(() => node.occupiedUser = isMine ? Definitions.MY_INDEX : Definitions.OTHER_INDEX);
            sequence.AppendInterval(0.5f);
            sequence.Append(node.transform.DOScale(originalScale, 0.5f));
            sequence.AppendCallback(() => node.transform.localScale = originalScale);
        }

        sequence.AppendCallback(() => SetIsAnimation(false));
        sequence.AppendCallback(() => endCallback?.Invoke());

        sequence.Play();
    }

    #endregion
}
