using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitHelper : MonoBehaviour
{
    [SerializeField] PVPGameManager PVPGameManagerPrefab;
    [SerializeField] PVEGameManager PVEGameManagerPrefab;
    [SerializeField] TestPVEGameManager TestGameManagerPrefab;

    private void Start()
    {
        var pve = ContentsDataManager.Instance.GetPVEGameInitData();
        var pvp = ContentsDataManager.Instance.GetPVPGameInitData();
        var test = ContentsDataManager.Instance.GetTestData();

        if (null != pve)
        {
            PVEGameManager gameManager = Instantiate(PVEGameManagerPrefab);
        }
        else if(null != pvp)
        {
            PhotonNetwork.Instantiate(Definitions.PVPGameManager, Vector3.zero, Quaternion.identity);
        }
        else if(null != test)
        {
            TestPVEGameManager gameManager = Instantiate(TestGameManagerPrefab);
        }
    }
}
