using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardDrawer
{
    public class InitData
    {
        public List<PhaseData> phaseDatas;
    }

    public static bool isDrawing;

    void Init(List<PhaseData> phaseDatas, ref List<ICard> myCards, ref List<ICard> otherCards, Action<ICard> nextTurnCallback);
    void DrawCard(bool isPlayerTurn, bool isTween = true);
}
