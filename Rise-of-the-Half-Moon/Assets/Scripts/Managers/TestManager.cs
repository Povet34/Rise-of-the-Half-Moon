using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : Singleton<TestManager>
{
    public bool isTest;

    public GameObject testCards;

    private void Start()
    {
        if (!Application.isEditor)
            isTest = false;
    }
}
