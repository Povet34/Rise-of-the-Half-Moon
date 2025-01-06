using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SFXData")]
public class SFXData : ScriptableObject
{
    public string soundName;
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
}