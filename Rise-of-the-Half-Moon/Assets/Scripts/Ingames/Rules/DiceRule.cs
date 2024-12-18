using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRule : ContentRule
{
    public override void Init()
    {
        base.Init();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool IsRemainScoreSettlement()
    {
        return base.IsRemainScoreSettlement();
    }

    public override void OnCardPlaced(Node node, bool isMine)
    {
        base.OnCardPlaced(node, isMine);
    }

    public override int PredictScoreForCard(Node node, ICard card)
    {
        return base.PredictScoreForCard(node, card);
    }

    public override void SetIsAnimation(bool isOn)
    {
        base.SetIsAnimation(isOn);
    }

    public override void SettlementOccupiedNodes(Action settlementEndCallback)
    {
        base.SettlementOccupiedNodes(settlementEndCallback);
    }

    public override string ToString()
    {
        return base.ToString();
    }

    protected override void AddAnimateQueue(bool isMine, List<Node> nodes, int score, Action endCallback = null)
    {
        base.AddAnimateQueue(isMine, nodes, score, endCallback);
    }

    protected override void AnimateNodes(List<Node> nodes, bool isMine, Action endCallback)
    {
        base.AnimateNodes(nodes, isMine, endCallback);
    }


    protected override void CheckAdjacentNodes(Node node, bool isMine)
    {
        base.CheckAdjacentNodes(node, isMine);
    }

    protected override void CheckCombineNodes(Node node, bool isMine)
    {
        base.CheckCombineNodes(node, isMine);
    }

    protected override void CheckCycle(Node node, bool isMine)
    {
        base.CheckCycle(node, isMine);
    }

    protected override IEnumerator DelayAnimation()
    {
        return base.DelayAnimation();
    }

    protected override bool IsCombination(int index1, int index2)
    {
        return base.IsCombination(index1, index2);
    }
}
