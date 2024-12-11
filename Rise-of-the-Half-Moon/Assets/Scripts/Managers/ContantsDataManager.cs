using System.Collections.Generic;

public class ContantsDataManager : Singleton<ContantsDataManager>
{
    public List<PhaseData> moonPhaseDatas = new List<PhaseData>();
    public List<PhaseData> numberPhaseDatas = new List<PhaseData>();
    public List<PhaseData> dicePhaseDatas = new List<PhaseData>();

    public List<PhaseData> GetPhaseDatas(PhaseData.ContentType content, ref ContentRule rule)
    {
        switch (content)
        {
            case PhaseData.ContentType.Moon:
                rule = gameObject.AddComponent<MoonRule>();
                return moonPhaseDatas;
            case PhaseData.ContentType.Dice:
                //rule = gameObject.AddComponent<DiceRule>();
                return dicePhaseDatas;
            case PhaseData.ContentType.Number:
                rule = gameObject.AddComponent<NumberRule>();
                return numberPhaseDatas;
            default:
                return null;
        }
    }
}
