using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public virtual ContentRule Rule => rule;
    public bool IsNetworkGame { get; protected set; }

    [Header("Data")]
    public bool isMyTurn;
    public PhaseData.ContentType contentType;
    protected ContentRule rule;
    protected System.Random random;
    protected NodeGenerator nodeGenerator;
    protected List<PhaseData> phaseDatas;
    protected GameUI gameUI;
    protected InGameCamController camController;
    protected GlobalVolumeController volumeController;
    protected ICardDrawer cardDrawer;

    [Header("Me")]
    protected int myScore;
    protected List<ICard> myCards = new List<ICard>();

    [Header("Other")]
    protected int otherScore;
    protected List<ICard> otherCards = new List<ICard>();

    protected virtual void Awake()
    {
        nodeGenerator = FindAnyObjectByType<NodeGenerator>();
        gameUI = FindAnyObjectByType<GameUI>();
        camController = FindAnyObjectByType<InGameCamController>();
        volumeController = FindAnyObjectByType<GlobalVolumeController>();
    }

    protected virtual void Start()
    {
        volumeController.SetSuperiorUserEffect(Color.white, 1, 1);
    }

    protected virtual void InitCardList()
    {
        foreach (var card in otherCards)
        {
            card.Destroy();
        }

        foreach (var card in myCards)
        {
            card.Destroy();
        }

        myCards.Clear();
        otherCards.Clear();
    }

    protected virtual void InitNodeGenerator()
    {
        nodeGenerator.Create();
    }

    protected virtual void InitRule(int type)
    {
        InitRule((PhaseData.ContentType)type);
    }

    protected virtual void InitRule(PhaseData.ContentType type)
    {
        contentType = type;
        phaseDatas = ContentsDataManager.Instance.GetPhaseDatas(contentType, ref rule);
        rule.Init();
    }

    protected virtual void InitCam() 
    {
        camController.Init(nodeGenerator.GetNodeBounds());
    }

    protected void InitCardDrawer()
    {
        cardDrawer.Init(phaseDatas, ref myCards, ref otherCards, NextTurn);
    }

    protected virtual void InitCards(int cardCount, List<ICard> cards, bool isPlayer1)
    {
        for (int i = 0; i < cardCount; i++)
        {
            cardDrawer.DrawCard(isPlayer1, false);
        }
    }

    protected virtual void NextTurn(ICard removedCard)
    {
        if (nodeGenerator.IsEndGame())
        {
            SettlementPlay();
            return;
        }

        //����Ʈ���� �����ϰ�
        if (isMyTurn)
            myCards.Remove(removedCard);
        else
            otherCards.Remove(removedCard);

        //���� �ٲ��ְ�
        isMyTurn = !isMyTurn;

        //��ο��Ѵ�
        cardDrawer.DrawCard(isMyTurn);
    }

    protected void SettlementPlay()
    {
        Rule.SettlementOccupiedNodes(
            () =>
            {
                if (myScore > otherScore)
                {
                    gameUI.ShowWin();
                }
                else if (myScore < otherScore)
                {
                    gameUI.ShowLose();
                }
                else
                {
                    gameUI.ShowDraw();
                }
            });
    }

    #region Update Score

    public virtual void UpdateMyScore(int score)
    {
        myScore += score;
        gameUI.UpdateMyScore(myScore);

        EffectUpdateScore();
    }
    public virtual void UpdateOtherScore(int score) 
    {
        otherScore += score;
        gameUI.UpdateOtherScore(otherScore);

        EffectUpdateScore();
    }

    protected virtual void EffectUpdateScore()
    {
        Color color = Color.white;
        if(myScore != otherScore)
            color = myScore > otherScore ? Color.red : Color.blue;

        float ratio = Mathf.InverseLerp(0, myScore + otherScore, Mathf.Abs(myScore - otherScore));
        float threshold = 1 - (ratio * 0.5f);

        volumeController.SetSuperiorUserEffect(color, threshold, 3);
    }

    public RectTransform GetScoreRt(bool isMine) 
    {
        return isMine ? gameUI.GetMyScoreUI : gameUI.GetOtherScoreUI;
    }

    public Vector3 GetMyProfileWorldPosition()
    {
        return gameUI.GetMyProfileWorldPosition();
    }

    public Vector3 GetOtherProfileWorldPosition()
    {
        return gameUI.GetOtherProfileWorldPosition();
    }

    #endregion
}
