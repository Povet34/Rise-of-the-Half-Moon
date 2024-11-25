using UnityEngine;

[CreateAssetMenu(fileName = "NewMoonPhaseData", menuName = "Game/MoonPhase")]

public class MoonPhaseData : ScriptableObject
{
    public enum PhaseType
    {
        NewMoon,            //��
        WaningCrescent,     //�׹ʴ�
        ThirdQuarter,       //������
        WaningGibbous,      //������
        FullMoon,           //������
        WaxingGibbous,      //������
        FirstQuarter,       //������
        WaxingCrescent      //�ʽ´�
    }

    public PhaseType phaseType;  
    public Texture2D phaseTexture;
    public Sprite cardBackSprite;
    public string description;

    public Sprite GetSprite(bool isMine)
    {
        if (phaseTexture == null) return null;

        if (isMine)
            return Sprite.Create(phaseTexture, new Rect(0, 0, phaseTexture.width, phaseTexture.height), new Vector2(0.5f, 0.5f));
        else
            return cardBackSprite;
    }
}