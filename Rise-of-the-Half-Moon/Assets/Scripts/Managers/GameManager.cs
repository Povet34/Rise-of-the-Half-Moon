using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : VolatilitySingleton<GameManager>
{
    public virtual ContentRule Rule => rule;
    public bool IsNetworkGame { get; protected set; }

    [Header("Data")]
    protected ContentRule rule;
    public bool isMyTurn;
    public PhaseData.ContentType contentType;
    protected System.Random random;
    protected NodeGenerator nodeGenerator;
    protected List<PhaseData> phaseDatas;
    protected GameUI gameUI;
    protected CardDrawer cardDrawer;

    [Header("Me")]
    protected int myScore;
    protected List<Card> myCards = new List<Card>();

    [Header("Other")]
    protected int otherScore;
    protected List<Card> otherCards = new List<Card>();

    protected virtual void Awake()
    {
        nodeGenerator = FindAnyObjectByType<NodeGenerator>();
        gameUI = FindAnyObjectByType<GameUI>();
        cardDrawer = FindAnyObjectByType<CardDrawer>();
    }

    public virtual void UpdateMyScore(int score)
    {

    }

    public virtual void UpdateOtherScore(int score)
    {

    }
}
