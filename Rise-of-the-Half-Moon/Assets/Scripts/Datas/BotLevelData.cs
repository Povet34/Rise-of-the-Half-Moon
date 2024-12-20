using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BotLevelData", menuName = "Game/Bot")]
public class BotLevelData : ScriptableObject
{
    [Tooltip("각 Node의 점수 정확도")]
    public int accuracy;

    [Tooltip("초기 랜덤하게 두는 횟수")]
    public int initRandomPutCount;

    [Tooltip("다음 PlaceCard 까지 딜레이 시간")]
    public int placeDelay;
}
