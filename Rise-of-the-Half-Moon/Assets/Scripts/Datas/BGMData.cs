using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/BGMData")]
public class BGMData : ScriptableObject
{
    public string playlistName;
    public List<AudioClip> clips;
    public float volume = 1f;
    public float pitch = 1f;
}