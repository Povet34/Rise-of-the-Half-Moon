using UnityEngine;

[CreateAssetMenu(fileName = "NewMoonPhaseData", menuName = "Game/MoonPhase")]

public class MoonPhaseData : ScriptableObject
{
    public enum PhaseType
    {
        None = -100,
        NewMoon = 0,            // ��
        WaningCrescent = 1,     // �׹ʴ�
        ThirdQuarter = 2,       // ������
        WaningGibbous = 3,      // ������
        FullMoon = 4,           // ������
        WaxingGibbous = 5,      // ������
        FirstQuarter = 6,       // ������
        WaxingCrescent = 7,     // �ʽ´�
        Count = 8,
    }

    public int phaseIndex;
    public PhaseType phaseType;  

    public Texture2D phaseTexture;
    public Sprite phaseSprite;
    
    public Sprite cardBackSprite;
    public string description;

    public Sprite GetSprite(bool isMine)
    {
        if (phaseTexture == null && phaseSprite == null) return null;

        if (phaseTexture)
        {
            if (isMine)
                return Sprite.Create(phaseTexture, new Rect(0, 0, phaseTexture.width, phaseTexture.height), new Vector2(0.5f, 0.5f));
            else
                return cardBackSprite;
        }

        if (phaseSprite)
        {
            if (isMine)
                return phaseSprite;
            else
                return cardBackSprite;
        }

        return null;
    }

    /// <summary>
    /// Phase�� �������� Ȯ��
    /// </summary>
    /// <param name="currentPhase"></param>
    /// <param name="comparePhase"></param>
    /// <returns></returns>
    public bool IsPhasing(PhaseType currentPhase, PhaseType comparePhase)
    {
        return comparePhase == GetPreviousPhaseType(currentPhase) || comparePhase == GetNextPhaseType(currentPhase);
    }

    public static PhaseType GetPreviousPhaseType(PhaseType currentType)
    {
        int currentIndex = (int)currentType;
        int previousIndex = (currentIndex - 1 + (int)PhaseType.Count) % (int)PhaseType.Count; 
        return (PhaseType)previousIndex;
    }

    public static PhaseType GetNextPhaseType(PhaseType currentType)
    {
        int currentIndex = (int)currentType;
        int nextIndex = (currentIndex + 1) % (int)PhaseType.Count;
        return (PhaseType)nextIndex;
    }
}