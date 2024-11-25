using UnityEngine;

[CreateAssetMenu(fileName = "NewMoonPhaseData", menuName = "Game/MoonPhase")]

public class MoonPhaseData : ScriptableObject
{
    public string phaseName;  
    public Texture2D phaseTexture;
    public string description;

    public Sprite GetSprite()
    {
        if (phaseTexture == null) return null;
        return Sprite.Create(phaseTexture, new Rect(0, 0, phaseTexture.width, phaseTexture.height), new Vector2(0.5f, 0.5f));
    }
}