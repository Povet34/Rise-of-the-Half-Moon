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

        if (!globalVolume.profile.TryGet<Bloom>(out bloom))
            Debug.LogError("Bloom effect not found in the volume profile!");

        if (!globalVolume.profile.TryGet<LensDistortion>(out lensDistortion))
            Debug.LogError("Lens Distortion effect not found in the volume profile!");
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

    public void SetSuperiorUserEffect(Color color, float threshold, float intensity)
    {
        if (bloom != null)
        {
            bloom.intensity.value = intensity;
            bloom.threshold.value = threshold;
            bloom.tint.value = color;
        }
    }
}