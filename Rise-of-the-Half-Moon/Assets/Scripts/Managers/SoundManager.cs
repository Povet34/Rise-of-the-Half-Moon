using System.Collections;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] SFXData[] sfxDatas;
    [SerializeField] BGMData[] bgmDatas;

    private Coroutine bgmCoroutine;

    public void PlaySFX(string name)
    {
        GameObject go = new GameObject(name + "SFX");
        AudioSource sfxSource = go.AddComponent<AudioSource>();
        SFXData sound = System.Array.Find(sfxDatas, s => s.soundName == name);
        if (sound != null)
        {
            sfxSource.clip = sound.clip;
            sfxSource.volume = sound.volume;
            sfxSource.pitch = sound.pitch;
            sfxSource.Play();
        }

        Destroy(go, sound.clip.length);
    }

    public void PlayBGM(string name)
    {
        BGMData bgm = System.Array.Find(bgmDatas, b => b.playlistName == name);
        if (bgm != null && bgm.clips.Count > 0)
        {
            if (bgmCoroutine != null)
            {
                StopCoroutine(bgmCoroutine);
            }
            bgmCoroutine = StartCoroutine(PlayBGMCoroutine(bgm));
        }
    }

    private IEnumerator PlayBGMCoroutine(BGMData bgm)
    {
        int clipIndex = 0;
        while (true)
        {
            bgmSource.clip = bgm.clips[clipIndex];
            bgmSource.volume = bgm.volume;
            bgmSource.pitch = bgm.pitch;
            bgmSource.Play();

            yield return new WaitForSeconds(bgmSource.clip.length);

            clipIndex = (clipIndex + 1) % bgm.clips.Count;
        }
    }

    public void StopBGM()
    {
        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
            bgmCoroutine = null;
        }
        bgmSource.Stop();
    }
}
