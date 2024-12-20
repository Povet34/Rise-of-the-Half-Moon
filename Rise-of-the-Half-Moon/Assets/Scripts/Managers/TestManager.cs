using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : Singleton<TestManager>
{
    public bool isTest;

    private void Start()
    {
        if (!Application.isEditor)
            isTest = false;
    }
}
