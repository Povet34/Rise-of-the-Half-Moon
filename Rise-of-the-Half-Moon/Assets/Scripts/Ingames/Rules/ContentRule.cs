using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;

public class ContentRule : MonoBehaviour
{
    public enum ShowType
    {
        None,
        SamePhase,
        Combination,
        Cycle,
        End,
    }

    public struct AnimData
    {
        public bool isMine;
        public List<Node> nodes;
        public int score;
        public Action endCallback;
        public ShowType showType;
    }

    protected bool isAnimating = false;
    public bool IsSettlementDone { get; protected set; }

    protected NodeGenerator nodeGenerator;
    protected Queue<Action> animationQueue = new Queue<Action>();
    protected GameManager gameManager;
    protected GlobalVolumeController volumeController;

    ScoreStar scoreStarPrefab;

    public virtual void Init()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        nodeGenerator = FindAnyObjectByType<NodeGenerator>();
        volumeController = FindAnyObjectByType<GlobalVolumeController>();
        scoreStarPrefab = Resources.Load<ScoreStar>("ScoreStar");

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
            else if(node.occupiedUser == Definitions.OTHER_INDEX)
            {
                otherOccupiedNodes.Add(node);
            }
        }

        AnimData data = new AnimData();
        data.isMine = true;
        data.nodes = myOccupiedNodes;
        data.score = Definitions.SETTLEMENT_SCORE;
        data.showType = ShowType.End;
        AddAnimateQueue(data);

        data = new AnimData();
        data.isMine = false;
        data.nodes = otherOccupiedNodes;
        data.score = Definitions.SETTLEMENT_SCORE;
        AddAnimateQueue(data);

        LastSettlementAnimate(settlementEndCallback);
    }

    public virtual void OnCardPlaced(Node node, bool isMine) 
    {
        CheckSameNodes(node, isMine);
        CheckCombineNodes(node, isMine);
        CheckCycle(node, isMine);
    }

    protected virtual void CheckSameNodes(Node node, bool isMine) 
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && adjacentNode.GetPhaseType() == node.GetPhaseType())
            {
                AnimData data = new AnimData();
                data.isMine = isMine;
                data.nodes = new List<Node>() { node, adjacentNode };
                data.score = Definitions.SAME_PHASE_SCORE;
                data.showType = ShowType.SamePhase;
                
                AddAnimateQueue(data);
            }
        }
    }

    protected virtual void CheckCombineNodes(Node node, bool isMine) 
    {
        foreach (Node adjacentNode in node.GetAdjacentNodes())
        {
            if (adjacentNode.phaseData != null && IsCombination(node.GetPhaseType(), adjacentNode.GetPhaseType()))
            {
                AnimData data = new AnimData();
                data.isMine = isMine;
                data.nodes = new List<Node>() { node, adjacentNode };
                data.score = Definitions.COMBINATION_SCORE;
                data.showType = ShowType.Combination;

                AddAnimateQueue(data);
            }
        }
    }

    protected virtual void CheckCycle(Node node, bool isMine)
    {
        List<List<Node>> cycles = nodeGenerator.GetSequentialPhaseNodes(node);

        foreach (var cycle in cycles)
        {
            AnimData data = new AnimData();
            data.isMine = isMine;
            data.nodes = cycle;
            data.score = Definitions.PHASE_CYCLE_SCORE;
            data.showType = ShowType.Cycle;

            AddAnimateQueue(data);
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

    protected virtual void AddAnimateQueue(AnimData data) 
    {
        animationQueue.Enqueue(() =>
        {
            AnimateNodes(data);
        });
    }

    protected virtual void LastSettlementAnimate(Action settlementEndCallback)
    {
        animationQueue.Enqueue(() =>
        {
            IsSettlementDone = true;
            settlementEndCallback.Invoke();
        });
    }

    protected virtual void AnimateNodes(AnimData data)
    {
        Color color = data.isMine ? Definitions.My_Occupied_Color : Definitions.Other_Occupied_Color;
        int userIndex = data.isMine ? Definitions.MY_INDEX : Definitions.OTHER_INDEX;
        Vector3 targetPos = data.isMine ? gameManager.GetMyProfileWorldTr().position : gameManager.GetOtherProfileWorldTr().position;

        Sequence sequence = DOTween.Sequence();
        SetIsAnimation(true);
        sequence.AppendCallback(() => gameManager.ShowMakePatternNotifier(data.showType.ToString()));

        switch (data.showType)
        {
            case ShowType.SamePhase:
            case ShowType.Combination:
                _AnimateAtOnce();
                break;
            case ShowType.Cycle:
            case ShowType.End:
                _AnimateSequentially();
                break;
        }

        sequence.AppendCallback(() => SetIsAnimation(false));
        sequence.AppendCallback(() => data.endCallback?.Invoke());
        sequence.Play();

        void _UpdateScore(bool isMine)
        {
            if (data.isMine)
                gameManager.UpdateMyScore(data.score);
            else
                gameManager.UpdateOtherScore(data.score);
        }

        /// 순차적으로 애니메이션
        void _AnimateSequentially()
        {
            sequence.AppendCallback(() => ScaleNodes(data.nodes, 1.5f, 0.2f, true));
            sequence.AppendCallback(() => SetEmissionColorNodes(data.nodes, Definitions.Non_Occupied_Color));
            sequence.AppendInterval(0.5f);

            foreach (Node node in data.nodes)
            {
                sequence.AppendCallback(() => node.EnableEmission(color));
                sequence.AppendCallback(() => node.SetOccupiedUser(userIndex));
                sequence.AppendCallback(() => _DoScoreEffect(node.transform, targetPos));

                sequence.AppendInterval(0.3f);
            }

            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => ScaleNodes(data.nodes, 1.0f, 0.2f, true));
            sequence.AppendCallback(() => ScaleNodes(data.nodes, 1.0f));
        }

        /// 한번에 애니메이션
        void _AnimateAtOnce()
        {
            sequence.AppendCallback(() => ScaleNodes(data.nodes, 1.5f, 0.2f, true));
            sequence.AppendCallback(() => SetEmissionColorNodes(data.nodes, Definitions.Non_Occupied_Color));
            sequence.AppendInterval(0.5f);

            foreach (Node node in data.nodes)
            {
                sequence.AppendCallback(() => node.EnableEmission(color));
                sequence.AppendCallback(() => node.SetOccupiedUser(userIndex));
            }

            var sharedEdge = data.nodes[0].GetSharedEdge(data.nodes[1]);
            _DoScoreEffect(sharedEdge.transform, targetPos);

            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => ScaleNodes(data.nodes, 1.0f, 0.2f, true));
            sequence.AppendCallback(() => ScaleNodes(data.nodes, 1.0f));
        }

        void _DoScoreEffect(Transform tr, Vector3 targetPos)
        {
            var st = Instantiate(scoreStarPrefab, tr.position, Quaternion.identity);
            st.DoEffect(new ScoreStar.Data()
            {
                isMine = data.isMine,
                targetPos = targetPos,
                endCallback = () => 
                {
                    _UpdateScore(data.isMine);
                }
            });
        }
    }

    protected virtual void ScaleNodes(List<Node> nodes, float scale, float duraction = 0f, bool isTween = false)
    {
        if (isTween)
        {
            foreach (Node node in nodes)
            {
                node.transform.DOScale(Vector3.one * scale, duraction);
            }
        }
        else
        {
            foreach (Node node in nodes)
            {
                node.transform.localScale = Vector3.one * scale;
            }
        }
    }

    protected virtual void SetEmissionColorNodes(List<Node> nodes, Color color)
    {
        foreach (Node node in nodes)
        {
            node.EnableEmission(color);
        }
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