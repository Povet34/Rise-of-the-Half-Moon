using UnityEngine;

[CreateAssetMenu(fileName = "NewMoonPhaseData", menuName = "Game/MoonPhase")]

public class MoonPhaseData : ScriptableObject
{
    public enum PhaseType
    {
        NewMoon,            //삭
        WaningCrescent,     //그믐달
        ThirdQuarter,       //하현달
        WaningGibbous,      //하현망
        FullMoon,           //보름달
        WaxingGibbous,      //상현망
        FirstQuarter,       //상현달
        WaxingCrescent      //초승달
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