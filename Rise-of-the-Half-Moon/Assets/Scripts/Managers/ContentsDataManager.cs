using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ContentsDataManager : Singleton<ContentsDataManager>
{
    public List<PhaseData> moonPhaseDatas = new List<PhaseData>();
    public List<PhaseData> numberPhaseDatas = new List<PhaseData>();
    public List<PhaseData> dicePhaseDatas = new List<PhaseData>();
    public List<BotLevelData> botLevelDatas = new List<BotLevelData>();

    public PVEGameManager.GameInitData gameInitData;

    public List<PhaseData> GetPhaseDatas(PhaseData.ContentType content, ref ContentRule rule)
    {
        switch (content)
        {
            case PhaseData.ContentType.Moon:
                rule = gameObject.AddComponent<MoonRule>();
                return moonPhaseDatas;
            case PhaseData.ContentType.Dice:
                rule = gameObject.AddComponent<DiceRule>();
                return dicePhaseDatas;
            case PhaseData.ContentType.Number:
                rule = gameObject.AddComponent<NumberRule>();
                return numberPhaseDatas;
            default:
                return null;
        }
    }

    public BotLevelData GetBotLevelData(int index)
    {
        return botLevelDatas[index];
    }
}
