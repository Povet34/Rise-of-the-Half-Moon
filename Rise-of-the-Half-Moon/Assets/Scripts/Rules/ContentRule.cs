using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ContentRule : MonoBehaviour
{
    protected bool isAnimating = false;
    protected NodeGenerator nodeGenerator;
    protected Queue<Action> animationQueue = new Queue<Action>();

    protected virtual void Awake()
    {
        nodeGenerator = FindObjectOfType<NodeGenerator>();
        StartCoroutine(DelayAnimation());
    }

    #region Calc Score

    public virtual void SettlementOccupiedNodes(Action settlementEndCallback) { }

    public virtual void OnCardPlaced(Node node, bool isMine) { }

    protected virtual void CheckAdjacentNodes(Node node, bool isMine) { }

    protected virtual void CheckFullMoon(Node node, bool isMine) { }

    protected virtual void CheckCycle(Node node, bool isMine) { }

    protected virtual bool IsCombination(int index1, int index2) { return false; }

    #endregion

    #region Infer Score

    public virtual int PredictScoreForCard(Node node, Card card)
    {
        return -1;
    }

    #endregion

    #region Animation

    protected virtual IEnumerator DelayAnimation() { yield break; }

    protected virtual void AddAnimateQueue(bool isMine, List<Node> nodes, int score, Action endCallback = null) { }

    protected virtual void AnimateNodes(List<Node> nodes, bool isMine, Action endCallback) { }

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