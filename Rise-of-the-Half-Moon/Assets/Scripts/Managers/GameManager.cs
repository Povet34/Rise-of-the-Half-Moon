using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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
    }

    public virtual void UpdateMyScore(int score) { }
    public virtual void UpdateOtherScore(int score) { }
}
