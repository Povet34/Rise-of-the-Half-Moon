using System;
using UnityEngine;

public interface ICard
{
    public struct CardParam
    {
        public Action<ICard> nextTurnCallback;
        public Action replaceCallback;

        public CardParam(Action<ICard> nextTurnCallback, Action replaceCallback)
        {
            this.nextTurnCallback = nextTurnCallback;
            this.replaceCallback = replaceCallback;
        }
    }

    public static bool IsDraging { get; set; }
    public bool IsMine { get; set; }
    public RectTransform rt { get; set; }
    public PhaseData phaseData { get; set; }
    void Init(CardParam param);
    void PlaceCard(Node node);
    void Destroy();
}
