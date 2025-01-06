using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreStar : MonoBehaviour
{
    public struct Data
    {
        public bool isMine;
        public Vector3 targetPos;
        public Action endCallback;
        public int spawnSoundPitch;
    }

    [SerializeField] SpriteRenderer bigStarRenderer;

    public void DoEffect(Data data)
    {
        SetColor(data);
        SetMoveSequene(data);
    }

    private void SetMoveSequene(Data data) 
    {
        Vector3 randomDirection = Random.insideUnitSphere * 3.0f;
        randomDirection.z = 0;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => SoundManager.Instance.PlaySFX(Definitions.SOUND_OCCUR_SCORE_STAR, data.spawnSoundPitch));
        sequence.Append(transform.DOMove(transform.position + randomDirection, 0.3f).SetEase(Ease.OutBounce));
        sequence.Append(transform.DOLocalMove(data.targetPos, 0.5f).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() => 
        {
            data.endCallback?.Invoke();
            Destroy(gameObject);
            SoundManager.Instance.PlayBGM(Definitions.SOUND_ARRIVE_SCORE_STAR);
        });


        sequence.Play();
    }

    private void SetColor(Data data)
    {
        bigStarRenderer.color = data.isMine ? Definitions.My_Occupied_Color : Definitions.Other_Occupied_Color;
    }
}
