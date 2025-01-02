using AssetKits.ParticleImage;
using UnityEngine;

public class ScoreStar : MonoBehaviour
{
    public void DoEffect(Vector2 startPos, RectTransform attractor)
    {
        ParticleImage pi = GetComponent<ParticleImage>();
        
        if (!pi)
            return;

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = startPos;

        pi.onFirstParticleFinished.AddListener(() =>
        {
            Destroy(gameObject);
        });

        pi.attractorTarget = attractor;
        pi.Play();
    }
}
