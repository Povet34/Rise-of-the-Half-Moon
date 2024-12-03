using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

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
                rule = gameObject.AddComponent<MoonRule>();
                return numberPhaseDatas;
            case PhaseData.ContentType.Number:
                rule = gameObject.AddComponent<MoonRule>();
                return dicePhaseDatas;
            default:
                return null;
        }
    }
}
