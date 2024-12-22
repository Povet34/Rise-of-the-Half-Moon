using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ContentsDataManager : Singleton<ContentsDataManager>
{
    public PhaseData.ContentType testType;
    public List<PhaseData> moonPhaseDatas = new List<PhaseData>();
    public List<PhaseData> numberPhaseDatas = new List<PhaseData>();
    public List<PhaseData> dicePhaseDatas = new List<PhaseData>();
    public List<BotLevelData> botLevelDatas = new List<BotLevelData>();

    private PVPGameManager.GameInitData pvpGameInitData;
    private PVEGameManager.GameInitData pveGameInitData;
    private PVEGameManager.GameInitData testGameData;

    public List<PhaseData> GetPhaseDatas(PhaseData.ContentType content, ref ContentRule rule)
    {
        switch (content)
        {
            case PhaseData.ContentType.Moon:
                rule = new GameObject(Definitions.MoonRule).AddComponent<MoonRule>();
                return moonPhaseDatas;
            case PhaseData.ContentType.Dice:
                rule = new GameObject(Definitions.DiceRule).AddComponent<DiceRule>();
                return dicePhaseDatas;
            case PhaseData.ContentType.Number:
                rule = new GameObject(Definitions.NumberRule).AddComponent<NumberRule>();
                return numberPhaseDatas;
            default:
                return null;
        }
    }

    public BotLevelData GetBotLevelData(int level)
    {
        if (level >= 0 && level < botLevelDatas.Count)
        {
            return botLevelDatas[level];
        }
        else
        {
            Debug.LogError($"Invalid level: {level}");
            return null;
        }
    }

    public void SetPVPGameInitData()
    {
        PhotonPlayerData myData = PhotonPlayerData.FromCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        PVPGameManager.GameInitData data = new PVPGameManager.GameInitData
        {
            contentType = (PhaseData.ContentType)Random.Range(0, (int)PhaseData.ContentType.Count),
            myPlayerData = myData,
            otherPlayerData = PhotonPlayerData.FromCustomProperties(PhotonNetwork.PlayerListOthers[0].CustomProperties),
            seed = Random.Range(0, 10000),
        };

        pvpGameInitData = data;
    }

    public PVPGameManager.GameInitData GetPVPGameInitData()
    {
        return pvpGameInitData;
    }

    public void SetPVEGameInitData()
    {
        PVEGameManager.GameInitData data = new PVEGameManager.GameInitData();
        data.contentType = (PhaseData.ContentType)Random.Range(0, (int)PhaseData.ContentType.Count);
        data.initBotLevel = Random.Range(0, ContentsDataManager.Instance.botLevelDatas.Count);
        data.seed = Random.Range(0, 100000);

        pveGameInitData = data;
    }

    public PVEGameManager.GameInitData GetPVEGameInitData()
    {
        return pveGameInitData;
    }

    public void SetTestData()
    {
        PVEGameManager.GameInitData data = new PVEGameManager.GameInitData();
        data.contentType = (PhaseData.ContentType)Random.Range(0, (int)PhaseData.ContentType.Count);
        data.initBotLevel = 0;
        data.seed = 1;

        testGameData = data;
    }

    public PVEGameManager.GameInitData GetTestData()
    {
        return testGameData;
    }

    public void ClearDatas()
    {
        pveGameInitData = null;
        pvpGameInitData = null;
    }

    public PhaseData GetPhaseData(PhaseData.ContentType type, int index)
    {
        List<PhaseData> phaseDataList = null;

        switch (type)
        {
            case PhaseData.ContentType.Dice:
                phaseDataList = dicePhaseDatas;
                break;
            case PhaseData.ContentType.Moon:
                phaseDataList = moonPhaseDatas;
                break;
            case PhaseData.ContentType.Number:
                phaseDataList = numberPhaseDatas;
                break;
            default:
                Debug.LogError($"Invalid ContentType: {type}");
                return null;
        }

        if (index >= 0 && index < phaseDataList.Count)
        {
            return phaseDataList[index];
        }
        else
        {
            Debug.LogError($"Invalid index: {index} for ContentType: {type}");
            return null;
        }
    }
}
