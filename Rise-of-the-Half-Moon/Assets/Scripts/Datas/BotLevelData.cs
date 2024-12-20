using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BotLevelData", menuName = "Game/Bot")]
public class BotLevelData : ScriptableObject
{
    [Tooltip("�� Node�� ���� ��Ȯ��")]
    public int accuracy;

    [Tooltip("�ʱ� �����ϰ� �δ� Ƚ��")]
    public int initRandomPutCount;

    [Tooltip("���� PlaceCard ���� ������ �ð�")]
    public int placeDelay;
}
