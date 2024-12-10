using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NumberRule : ContentRule
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
        CheckNumberCombination(node, isMine);
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

    protected void CheckNumberCombination(Node node, bool isMine)
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && IsCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                AddAnimateQueue(isMine, new List<Node>() { node, adjacentNode }, Definitions.NUMBER_COMBINATION_SCORE);
            }
        }
    }

    protected override bool IsCombination(int index1, int index2)
    {
        // index1과 index2가 특정 조합을 이루는지 확인하여 true를 반환
        return (index1 + index2) % 2 == 0; // 예시: 두 숫자의 합이 짝수인 경우
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

        // Check adjacent nodes for number combination
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && IsCombination(card.phaseData.phaseIndex, adjacentNode.GetPhaseType()))
            {
                totalScore += Definitions.NUMBER_COMBINATION_SCORE;
            }
        }

        return totalScore;
    }

    #endregion

    #region Animation

    protected override IEnumerator DelayAnimation()
    {
        while (true)
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
                GameManager.Instance.UpdateMyScore(score);
            else
                GameManager.Instance.UpdateOtherScore(score);
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
