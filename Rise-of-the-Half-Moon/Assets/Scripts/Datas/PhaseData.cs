using UnityEngine;

[CreateAssetMenu(fileName = "PhaseData", menuName = "Game/Phase")]

public class PhaseData : ScriptableObject
{
    public enum ContentType
    {
        Dice,
        Moon,
        Number,

        Count,
    }

    public ContentType contentType;
    public int phaseIndex;

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

    public Texture2D GetTexture()
    {
        if (phaseTexture == null && phaseSprite == null) return null;

        if(phaseTexture)
        {
            return phaseTexture;
        }
        else if(phaseSprite)
        {
            Texture2D texture = new Texture2D((int)phaseSprite.rect.width, (int)phaseSprite.rect.height);
            Color[] pixels = phaseSprite.texture.GetPixels((int)phaseSprite.textureRect.x, (int)phaseSprite.textureRect.y, (int)phaseSprite.textureRect.width, (int)phaseSprite.textureRect.height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        return null;
    }

    /// <summary>
    /// Phase의 영역인지 확인
    /// </summary>
    /// <param name="currentPhase"></param>
    /// <param name="comparePhase"></param>
    /// <returns></returns>
    public static bool IsPhasing(int currentPhase, int comparePhase, ContentType type)
    {
        return comparePhase == GetPreviousPhaseType(currentPhase, type) || comparePhase == GetNextPhaseType(currentPhase, type);
    }

    public static int GetPreviousPhaseType(int currentType, ContentType type)
    {
        int currentIndex = (int)currentType;
        int previousIndex = (currentIndex - 1 + GetContentDataCount(type)) % GetContentDataCount(type); 
        return previousIndex;
    }

    public static int GetNextPhaseType(int currentType, ContentType type)
    {
        int currentIndex = (int)currentType;
        int nextIndex = (currentIndex + 1) % GetContentDataCount(type);
        return nextIndex;
    }

    static int GetContentDataCount(ContentType type)
    {
        return type switch
        {
            ContentType.Dice => Definitions.DiceDataCount,
            ContentType.Moon => Definitions.MoonPhaseDataCount,
            ContentType.Number => Definitions.NumberDataCount,
            _ => 0,
        };
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    // 직렬화된 데이터를 PhaseData 객체로 변환하는 메서드
    public static PhaseData Deserialize(string jsonData)
    {
        return JsonUtility.FromJson<PhaseData>(jsonData);
    }
}