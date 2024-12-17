using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : VolatilitySingleton<GameManager>
{

    protected ContentRule rule;
    public virtual ContentRule Rule => rule;

    [Header("Data")]
    public bool isMyTurn;
    public PhaseData.ContentType contentType;
    protected System.Random random;

    public virtual void UpdateMyScore(int score)
    {

    }

    public virtual void UpdateOtherScore(int score)
    {

    }
}
