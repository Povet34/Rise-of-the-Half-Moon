using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class ContentRule : MonoBehaviour
{
    protected bool isAnimating = false;
    protected bool isSettlementDone = false;

    protected NodeGenerator nodeGenerator;
    protected Queue<Action> animationQueue = new Queue<Action>();
    protected GameManager gameManager;
    protected GlobalVolumeController volumeController;

    public virtual void Init()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        nodeGenerator = FindAnyObjectByType<NodeGenerator>();
        volumeController = FindAnyObjectByType<GlobalVolumeController>();
        StartCoroutine(DelayAnimation());
    }

    #region Calc Score

    public virtual void SettlementOccupiedNodes(Action settlementEndCallback)
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
        LastSettlementAnimate();
    }

    public virtual void OnCardPlaced(Node node, bool isMine) 
    {
        CheckAdjacentNodes(node, isMine);
        CheckCombineNodes(node, isMine);
        CheckCycle(node, isMine);
    }

    protected virtual void CheckAdjacentNodes(Node node, bool isMine) 
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && adjacentNode.GetPhaseType() == node.GetPhaseType())
            {
                AddAnimateQueue(isMine, new List<Node>() { node, adjacentNode }, Definitions.SAME_PHASE_SCORE);
            }
        }
    }

    protected virtual void CheckCombineNodes(Node node, bool isMine) 
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && IsCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                AddAnimateQueue(isMine, new List<Node>() { node, adjacentNode }, Definitions.COMBINATION_SCORE);
            }
        }
    }

    protected virtual void CheckCycle(Node node, bool isMine)
    {
        List<List<Node>> cycles = nodeGenerator.GetSequentialPhaseNodes(node);

        foreach (var cycle in cycles)
        {
            AddAnimateQueue(isMine, cycle, Definitions.PHASE_CYCLE_SCORE * cycle.Count);
        }
    }

    protected virtual bool IsCombination(int index1, int index2) { return false; }

    #endregion

    #region Infer Score

    /// <summary>
    /// Bot이 현재의 Node상황에서 점수를 예측
    /// </summary>
    /// <param name="node"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public virtual int PredictScoreForCard(Node node, ICard card)
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
                totalScore += Definitions.COMBINATION_SCORE;
            }
        }

        // Check for moon cycle
        List<List<Node>> cycles = nodeGenerator.GetSequentialPhaseNodes(node);
        if (cycles.Count > 0)
        {
            foreach (var cycle in cycles)
                totalScore += Definitions.PHASE_CYCLE_SCORE * cycle.Count;
        }

        return totalScore;
    }

    #endregion

    #region Animation

    protected virtual IEnumerator DelayAnimation() 
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

    protected virtual void AddAnimateQueue(bool isMine, List<Node> nodes, int score, Action endCallback = null) 
    {
        animationQueue.Enqueue(() =>
        {
            AnimateNodes(nodes, isMine, endCallback);

            if (isMine)
                gameManager.UpdateMyScore(score);
            else
                gameManager.UpdateOtherScore(score);
        });
    }

    protected virtual void LastSettlementAnimate()
    {
        animationQueue.Enqueue(() =>
        {
            isSettlementDone = true;
        });
    }

    protected virtual void AnimateNodes(List<Node> nodes, bool isMine, Action endCallback) 
    {
        Sequence sequence = DOTween.Sequence();
        SetIsAnimation(true);

        foreach (Node node in nodes)
        {
            Vector3 originalScale = node.transform.localScale;
            Vector3 targetScale = originalScale * 1.5f;

            sequence.Append(node.transform.DOScale(targetScale, 0.2f));
            sequence.AppendCallback(() => node.EnableEmission(isMine ? Definitions.My_Occupied_Color : Definitions.Other_Occupied_Color));
            sequence.AppendCallback(() => node.occupiedUser = isMine ? Definitions.MY_INDEX : Definitions.OTHER_INDEX);
            sequence.AppendInterval(0.2f);
            sequence.Append(node.transform.DOScale(originalScale, 0.2f));
            sequence.AppendCallback(() => node.transform.localScale = originalScale);
        }

        sequence.AppendCallback(() => SetIsAnimation(false));
        sequence.AppendCallback(() => endCallback?.Invoke());

        sequence.Play();
    }

    public virtual bool IsRemainScoreSettlement()
    {
        return animationQueue.Count > 0 || isAnimating;
    }

    public virtual void SetIsAnimation(bool isOn)
    {
        isAnimating = isOn;
    }

    #endregion
}