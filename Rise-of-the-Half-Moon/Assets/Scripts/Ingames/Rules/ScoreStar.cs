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
    }


    [SerializeField] SpriteRenderer bigStarRenderer;

    public void DoEffect(Data data)
    {
        SetColor(data.isMine);
        SetMoveSequene(data.targetPos, data.endCallback);
    }

    private void SetMoveSequene(Vector3 targetPos, Action endCallback) 
    {
        Vector3 randomDirection = Random.insideUnitSphere * 3.0f;
        randomDirection.z = 0;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(transform.position + randomDirection, 0.3f).SetEase(Ease.OutBounce));
        sequence.Append(transform.DOLocalMove(targetPos, 0.5f).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() => 
        {
            endCallback?.Invoke();
            Destroy(gameObject); 
        });


        sequence.Play();
    }

    private void SetColor(bool isMine)
    {
        bigStarRenderer.color = isMine ? Definitions.My_Occupied_Color : Definitions.Other_Occupied_Color;
    }
}
