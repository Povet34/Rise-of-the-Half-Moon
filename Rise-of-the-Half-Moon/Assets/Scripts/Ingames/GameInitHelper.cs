using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitHelper : MonoBehaviour
{
    [SerializeField] PVPGameManager PVPGameManagerPrefab;
    [SerializeField] PVEGameManager PVEGameManagerPrefab;

    private void Start()
    {
        var pve = ContentsDataManager.Instance.GetPVEGameInitData();
        var pvp = ContentsDataManager.Instance.GetPVPGameInitData();

        if (null != pve)
        {
            PVEGameManager gameManager = Instantiate(PVEGameManagerPrefab);
            gameManager.GameInit(pve);
        }
        else if(null != pvp)
        {

        }

        ContentsDataManager.Instance.ClearDatas();
    }
}
