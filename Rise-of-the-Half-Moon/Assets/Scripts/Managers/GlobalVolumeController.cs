using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeController : MonoBehaviour
{
    private Volume globalVolume;
    private Bloom bloom;
    private LensDistortion lensDistortion;

    // Start is called before the first frame update
    void Start()
    {
        globalVolume = GetComponent<Volume>();
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume component not found!");
            return;
        }

        // 볼륨 프로필에서 Bloom 효과를 가져옵니다.
        if (globalVolume.profile.TryGet<Bloom>(out bloom))
        {
            Debug.Log("Bloom effect found!");
        }
        else
        {
            Debug.LogError("Bloom effect not found in the volume profile!");
        }

        // 볼륨 프로필에서 Lens Distortion 효과를 가져옵니다.
        if (globalVolume.profile.TryGet<LensDistortion>(out lensDistortion))
        {
            Debug.Log("Lens Distortion effect found!");
        }
        else
        {
            Debug.LogError("Lens Distortion effect not found in the volume profile!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 예시: 키 입력으로 Bloom 효과의 intensity를 애니메이션으로 변경
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetBloomIntensity(5.0f, 1.0f); // intensity를 5.0으로 1초 동안 변경
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetBloomIntensity(100.0f, 1.0f); // intensity를 10.0으로 1초 동안 변경
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetLensDistortion(-1.0f, 0.01f, 1.0f); // intensity를 -1로, scale을 0.01로 1초 동안 변경
        }
    }

    public void SetBloomIntensity(float targetIntensity, float duration)
    {
        if (bloom != null)
        {
            DOTween.To(() => bloom.intensity.value, x => bloom.intensity.value = x, targetIntensity, duration);
        }
    }

    public void SetLensDistortion(float targetIntensity, float targetScale, float duration)
    {
        if (lensDistortion != null)
        {
            DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, targetIntensity, duration);
            DOTween.To(() => lensDistortion.scale.value, x => lensDistortion.scale.value = x, targetScale, duration * 2F);
        }
    }
}