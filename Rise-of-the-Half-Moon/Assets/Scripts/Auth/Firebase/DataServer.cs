using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;

public class DataServer : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public string name;

    }

    public Data userData;
    public string userId;
    DatabaseReference dbRef;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(userData);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    IEnumerator LoadData()
    {
        var serverData = dbRef.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);   

        Debug.Log("process is complete");

        if (serverData.Exception != null)
        {
            Debug.LogWarning(serverData.Exception.Message);
        }
        else
        {
            Data data = JsonUtility.FromJson<Data>(serverData.Result.GetRawJsonValue());
            userData = data;
        }   
    }
}
