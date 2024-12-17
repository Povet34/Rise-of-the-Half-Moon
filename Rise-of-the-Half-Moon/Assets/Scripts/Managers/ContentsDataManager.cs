using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContentsDataManager : Singleton<ContentsDataManager>
{
    public List<PhaseData> moonPhaseDatas = new List<PhaseData>();
    public List<PhaseData> numberPhaseDatas = new List<PhaseData>();
    public List<PhaseData> dicePhaseDatas = new List<PhaseData>();
    public List<BotLevelData> botLevelDatas = new List<BotLevelData>();

    public List<PhaseData> GetPhaseDatas(PhaseData.ContentType content, ref ContentRule rule)
    {
        switch (content)
        {
            case PhaseData.ContentType.Moon:
                rule = new GameObject("MoonRule").AddComponent<MoonRule>();
                return moonPhaseDatas;
            case PhaseData.ContentType.Dice:
                rule = new GameObject("DiceRule").AddComponent<DiceRule>();
                return dicePhaseDatas;
            case PhaseData.ContentType.Number:
                rule = new GameObject("NumberRule").AddComponent<NumberRule>();
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
