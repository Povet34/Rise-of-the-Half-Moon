using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoonRule : ContentRule
{

    #region Calc Score

    public override void SettlementOccupiedNodes(Action settlementEndCallback)
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


    public override void OnCardPlaced(Node node, bool isMine)
    {
        CheckAdjacentNodes(node, isMine);
        CheckCombineNodes(node, isMine);
        CheckCycle(node, isMine);
    }

    protected override void CheckAdjacentNodes(Node node, bool isMine)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && adjacentNode.GetPhaseType() == node.GetPhaseType())
            {
                AddAnimateQueue(isMine, new List<Node>() { node, adjacentNode }, Definitions.SAME_PHASE_SCORE);
            }
        }
    }

    protected override void CheckCombineNodes(Node node, bool isMine)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && IsCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                AddAnimateQueue(isMine, new List<Node>() { node, adjacentNode }, Definitions.FULL_MOON_SCORE);
            }
        }
    }

    protected override void CheckCycle(Node node, bool isMine)
    {
        List<List<Node>> cycles = nodeGenerator.GetSequentialPhaseNodes(node);

        foreach (var cycle in cycles)
        {
            AddAnimateQueue(isMine, cycle, Definitions.PHASE_CYCLE_SCORE);
        }
    }

    protected override bool IsCombination(int index1, int index2)
    {
        // index1과 index2가 특정 조합을 이루는지 확인하여 true를 반환
        return (index1 == 0 && index2 == 4) || // NewMoon (0) - FullMoon (4)
               (index1 == 4 && index2 == 0) || // FullMoon (4) - NewMoon (0)
               (index1 == 1 && index2 == 5) || // WaningCrescent (1) - WaxingGibbous (5)
               (index1 == 5 && index2 == 1) || // WaxingGibbous (5) - WaningCrescent (1)
               (index1 == 2 && index2 == 6) || // ThirdQuarter (2) - FirstQuarter (6)
               (index1 == 6 && index2 == 2) || // FirstQuarter (6) - ThirdQuarter (2)
               (index1 == 3 && index2 == 7) || // WaningGibbous (3) - WaxingCrescent (7)
               (index1 == 7 && index2 == 3);   // WaxingCrescent (7) - WaningGibbous (3)
    }

    #endregion

    #region Infer Score

    public override int PredictScoreForCard(Node node, Card card)
    {
        int totalScore = 0;

        // Check adjacent nodes for same phase type
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && adjacentNode.GetPhaseType() == card.phaseData.phaseIndex)
            {
                totalScore += Definitions.SAME_PHASE_SCORE;
            }
        }

        // Check adjacent nodes for full moon combination
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && IsCombination(card.phaseData.phaseIndex, adjacentNode.GetPhaseType()))
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

    protected override IEnumerator DelayAnimation()
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

    protected override void AddAnimateQueue(bool isMine, List<Node> nodes, int score, Action endCallback = null)
    {
        animationQueue.Enqueue(() => 
        {
            AnimateNodes(nodes, isMine, endCallback);

            if (isMine)
                PVEGameManager.Instance.UpdateMyScore(score);
            else
                PVEGameManager.Instance.UpdateOtherScore(score);
        });
    }

    protected override void AnimateNodes(List<Node> nodes, bool isMine, Action endCallback)
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
