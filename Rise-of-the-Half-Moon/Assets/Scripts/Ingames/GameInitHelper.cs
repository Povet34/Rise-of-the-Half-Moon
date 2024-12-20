using Photon.Pun;
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
        }
        else if(null != pvp)
        {
            PhotonNetwork.Instantiate("PVPGameManager", Vector3.zero, Quaternion.identity);
        }
    }
}
